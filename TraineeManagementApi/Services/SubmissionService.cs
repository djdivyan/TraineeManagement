using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Models;
using TraineeManagementApi.DTOs;
using TraineeManagementApi.Exceptions;
using TraineeManagementApi.Models;
using TraineeManagementApi.Utilities;
using TraineeManagementApi.Contracts;

namespace TraineeManagementApi.Services
{
    class SubmissionService(AppDbContext dbContext, ILogger<SubmissionService> logger, IFileStorageService fileStorageService, IWebHostEnvironment environment, ICacheService cacheService, IRabbitMqPublisher rabbitMqPublisher) : ISubmissionService
    {
        private readonly AppDbContext _dbContext = dbContext;
        private readonly ILogger<SubmissionService> _logger = logger;
        private readonly IWebHostEnvironment _env = environment;
        private readonly ICacheService _cache = cacheService;
        private readonly IFileStorageService _fileManager = fileStorageService;
        private readonly IRabbitMqPublisher _publisher = rabbitMqPublisher;
        private const string QueName = "submission-processing";

        public async Task<List<SubmissionResponse>> GetAllAsync()
        {
            _logger.LogInformation("GetAllAsync:Submission : Entering the Function");            
            IQueryable<Submission> query = _dbContext.Submissions.AsNoTracking();
                                                        // .Include(ta => ta.TaskAssignment);

            List<Submission> submissions = await query.ToListAsync();
            _logger.LogInformation("GetAllAsync:Submission : Successfully returned all Submissions");
            return submissions.Select(MapToResponse).ToList();
        }

        public async Task<SubmissionResponse> GetByIdAsync(int id)
        {
            _logger.LogInformation("GetByIdAsync:Submission : Entering the Function");            
            Submission? submission = await _dbContext.Submissions.FindAsync(id);
            if (submission is null)
            {
                _logger.LogError("GetByIdAsync:Submission : Submission Not found with {id}", id);
                throw new NotFoundException("Submission",id);
            }
            _logger.LogInformation("GetByIdAsync:Submission : TSubmission found with {id}", id);
            return MapToResponse(submission);
        }

        public async Task<SubmissionResponse> CreateAsync(SubmissionRequest submissionRequest)
        {
            _logger.LogInformation("CreateAsync:Submission : Entering the Function");            
            if (await _dbContext.TaskAssignments.FindAsync(submissionRequest.TaskAssignmentId) == null)
            {
                throw new BadRequestException($"Foreign Key - TaskAssignmentId : {submissionRequest.TaskAssignmentId} Does not Exist");
            }
            
            Submission submission = new()
            {
                TaskAssignmentId = submissionRequest.TaskAssignmentId,
                SubmissionUrl = submissionRequest.SubmissionUrl,
                Notes = submissionRequest.Notes,
                SubmittedDate = DateTime.Now,
                SubmissionStatus = submissionRequest.SubmissionStatus,
            };

            _dbContext.Submissions.Add(submission);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("CreateAsync:Submission : New Submission created with id {id} ", submission.Id);
            return MapToResponse(submission);
        }

        public static SubmissionResponse MapToResponse(Submission submission)
        {
            return new SubmissionResponse
            {
                Id = submission.Id,
                TaskAssignmentId = submission.TaskAssignmentId,
                // TaskAssignment = submission.TaskAssignment,
                SubmissionUrl = submission.SubmissionUrl,
                Notes = submission.Notes,
                SubmittedDate = submission.SubmittedDate,
                SubmissionStatus = submission.SubmissionStatus,
            };
        }

        public async Task<SubmissionFileResponseDTO> SaveFileAsync(int submissionId,SubmissionFileRequestDTO request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("SaveFileAsync:Submission : Entering the Function");            
            if (await _dbContext.Submissions.FindAsync(submissionId) == null)
            {
                throw new BadRequestException($"Foreign Key - SubmissionId : {submissionId} Does not Exist");
            }
            
            string originalFileName = request.File.FileName;
            //Saving File in Storage
            _logger.LogInformation("SaveFileAsync:Submission : Entering the Storage");            
            string generatedStorageName = await _fileManager.SaveAsync(request.File);
            _logger.LogInformation("SaveFileAsync:Submission : Generated StorageName {StorageName} ", generatedStorageName);            

            string contentType = request.File.ContentType;
            long size = request.File.Length;
            int uploadedByUser = request.UploadedByUser;
            int submissionid = submissionId;
            DateTime Timestamp = DateTime.Now;
            _logger.LogInformation("SaveFileAsync:Submission : Entering Checksum");            
            string checksum = GenerateChecksum(generatedStorageName);
            _logger.LogInformation("SaveFileAsync:Submission :  Checksum {checksum}", checksum);            

            SubmissionFile submissionFile = new SubmissionFile
            {
               SubmissionId = submissionid,
               OriginalFileName = originalFileName,
               GeneratedStorageName = generatedStorageName,
               ContentType = contentType,
               Size = size,
               Checksum = checksum,
               UploadedByUser = uploadedByUser,
               Timestamp = Timestamp
            };

            _dbContext.SubmissionFiles.Add(submissionFile);
            await _dbContext.SaveChangesAsync();
            

            //For Publishing message to the queue
            _logger.LogInformation("SaveFileAsync:Submission : Publishing Message to RabbitQueue");
            SubmissionProcessingRequested message = new()
            {
                MessageId = Guid.NewGuid(),
                SubmissionId = submissionFile.SubmissionId,
                CorrelationId = Guid.NewGuid(),
                FileId = submissionFile.Id,
                RequestedAt = DateTime.UtcNow
            };
            await _publisher.PublishAsync(QueName, message, cancellationToken);
            
            //Adding Job to the Queue
            ProcessingJob processingJob = new()
            {
                // Id = message.MessageId,
                CorrelationId = message.CorrelationId,
                Attempts = 0,
                StartedAt = DateTime.UtcNow,
                ProcessingJobStatus = ProcessingJobStatus.Queued
            }; 
            
            _dbContext.ProcessingJobs.Add(processingJob);
            await _dbContext.SaveChangesAsync();
            
            _logger.LogInformation("SaveFileAsync:Submission : New Submission File Created {submision}",JsonSerializer.Serialize(submissionFile));            
            return MapToFileMetadata(submissionFile,message.MessageId);
        }

        private SubmissionFileResponseDTO MapToFileMetadata(SubmissionFile request,Guid messageId)
        {
            return new SubmissionFileResponseDTO
            {
                TrackingIdentifier = messageId,
                Id = request.Id,
                SubmissionId = request.SubmissionId,
                OriginalFileName = request.OriginalFileName,
                GeneratedStorageName = request.GeneratedStorageName,
                ContentType = request.ContentType,
                Size = request.Size,
                Checksum = request.Checksum,
                UploadedByUser = request.UploadedByUser,
                Timestamp = request.Timestamp
            };
        }

        private string GenerateChecksum(string filename)
        {
            string contentPath = _env.ContentRootPath;
            string path = Path.Combine(contentPath, $"Uploads", filename);
            using (MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                using (FileStream stream = System.IO.File.OpenRead(path))
                {
                    byte[] hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "");
                }
            }
        }

        public async Task<SubmissionSummaryDTO> GetSubmissionSummaryAsync(int id, CancellationToken cancellationToken=default)
        {
            _logger.LogInformation("Submission:GetSubmissionSummaryAsync : Entering Function");

            string cacheKey = CacheKeys.SubmissionSummary(id);
            SubmissionSummaryDTO? submissionSummary = await _cache.GetOrSetAsync
            (
                cacheKey,
                async() =>
                {
                    _logger.LogInformation("Cache miss for {cacheKey}", cacheKey);
                    Submission? submission = await _dbContext.Submissions.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id,cancellationToken);
                    if (submission is null)
                    {
                        _logger.LogWarning("Submission:GetSubmissionSummaryAsync : Submission Not found with {id}", id);
                        throw new NotFoundException("Submission",id);
                    }

                    return new SubmissionSummaryDTO
                    {
                        TaskAssignmentId = submission.TaskAssignmentId,
                        Notes = submission.Notes,
                        SubmissionStatus = submission.SubmissionStatus,
                        SubmissionUrl = submission.SubmissionUrl,
                        SubmittedDate = submission.SubmittedDate
                    };
                },
                cancellationToken
            );



            if(submissionSummary is null)
            {
                throw new NotFoundException("SubmissionSummary",id);
            }

            _logger.LogInformation("Submission:GetSubmissionSummaryAsync : Returning Submission summary for ID {id}",id);
            return submissionSummary;
        }
    }
}
