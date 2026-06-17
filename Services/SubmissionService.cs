using Microsoft.EntityFrameworkCore;
using Models;
using TraineeManagementApi.DTOs;
using TraineeManagementApi.Exceptions;
using TraineeManagementApi.Models;

namespace TraineeManagementApi.Services
{
    class SubmissionService(AppDbContext dbContext, ILogger<SubmissionService> logger) : ISubmissionService
    {
        private readonly AppDbContext _dbContext = dbContext;
        private readonly ILogger<SubmissionService> _logger = logger;

        public async Task<List<SubmissionResponse>> GetAllAsync()
        {
            IQueryable<Submission> query = _dbContext.Submissions.AsNoTracking();
                                                        // .Include(ta => ta.TaskAssignment);

            List<Submission> submissions = await query.ToListAsync();
            _logger.LogInformation("GetAllAsnc Successfully returned all Submissions");
            return submissions.Select(MapToResponse).ToList();
        }

        public async Task<SubmissionResponse> GetByIdAsync(int id)
        {
            Submission? submission = await _dbContext.Submissions.FindAsync(id);
            if (submission is null)
            {
                _logger.LogError("GetByID : Submission Not found with {id}", id);
                throw new NotFoundException("Submission",id);
            }
            _logger.LogInformation("GetByID : TSubmission found with {id}", id);
            return MapToResponse(submission);
        }

        public async Task<SubmissionResponse> CreateAsync(SubmissionRequest submissionRequest)
        {
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

            _logger.LogInformation("Create : New Submission created with id {id} ", submission.Id);
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
    }
}
