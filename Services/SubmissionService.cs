using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Models;
using TraineeManagementApi.DTOs;
using TraineeManagementApi.Exceptions;
using TraineeManagementApi.Models;

namespace TraineeManagementApi.Services
{
    class SubmissionService(AppDbContext dbContext, ILogger<SubmissionService> logger, IFileStorageService fileStorageService, IWebHostEnvironment environment) : ISubmissionService
    {
        private readonly AppDbContext _dbContext = dbContext;
        private readonly ILogger<SubmissionService> _logger = logger;
        private readonly IWebHostEnvironment _env = environment;

         private readonly IFileStorageService _fileManager = fileStorageService;

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

        public async Task<SubmissionFileResponseDTO> SaveFileAsync(int submissionId,SubmissionFileRequestDTO request)
        {
            _logger.LogInformation("SaveFileAsync:Submission : Entering the Function");            
            if (await _dbContext.Submissions.FindAsync(submissionId) == null)
            {
                throw new BadRequestException($"Foreign Key - SubmissionId : {submissionId} Does not Exist");
            }
            
            string originalFileName = request.File.FileName;
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
            _logger.LogInformation("SaveFileAsync:Submission : New Submission File Created {submision}",JsonSerializer.Serialize(submissionFile));            
            return MapToFileMetadata(submissionFile);
        }

        private SubmissionFileResponseDTO MapToFileMetadata(SubmissionFile request)
        {
            return new SubmissionFileResponseDTO
            {
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
            var contentPath = _env.ContentRootPath;
            var path = Path.Combine(contentPath, $"Uploads", filename);
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                using (var stream = System.IO.File.OpenRead(path))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "");
                }
            }
        }
    }
}
