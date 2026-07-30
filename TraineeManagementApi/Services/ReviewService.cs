using Microsoft.EntityFrameworkCore;
using Models;
using TraineeManagementApi.DTOs;
using TraineeManagementApi.Exceptions;
using TraineeManagement.Shared.Models;

namespace TraineeManagementApi.Services
{
    class ReviewService(AppDbContext dbContext, ILogger<ReviewService> logger, IHttpContextAccessor httpContextAccessor) : IReviewService
    {
        private readonly AppDbContext _dbContext = dbContext;
        private readonly ILogger<ReviewService> _logger = logger;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task<Review?> GetRawByIdAsync(int id)
        {
            return await _dbContext.Reviews
                .Include(r => r.Submission)
                    .ThenInclude(s => s.TaskAssignment)
                        .ThenInclude(t => t.Trainee)
                .FirstOrDefaultAsync(r => r.Id == id);
        }


        public async Task<List<ReviewResponse>> GetAllAsync()
        {
            _logger.LogInformation("GetAllAsync:Review : Entering the Function");
            IQueryable<Review> query = _dbContext.Reviews.AsNoTracking();

            List<Review> reviews = await query.ToListAsync();
            _logger.LogInformation("GetAllAsync:Review :  Successfully returned all Reviews");
            return reviews.Select(MapToResponse).ToList();
        }

        public async Task<ReviewResponse> GetByIdAsync(int id)
        {
            _logger.LogInformation("GetByIdAsync:Review : Entering the Function");            

            Review? review = await GetRawByIdAsync(id);

            if (review is null)
            {
                _logger.LogError("GetByIdAsync:Review : Review Not found with {id}", id);
                throw new NotFoundException("Review",id);
            }

            _logger.LogInformation("GetByIdAsync:Review : Review found with {id}", id);
            return MapToResponse(review);
        }

        public async Task<ReviewResponse> CreateAsync(ReviewRequest reviewRequest)
        {
            _logger.LogInformation("CreateAsync:Review : Entering the Function");

            //Auth Check for create review
            if (!CheckAuthorization(reviewRequest.MentorId))
            {
                _logger.LogWarning("Forbidden Review Creation: User tried acting as Mentor {reqId}", reviewRequest.MentorId);
                throw new ForbiddenException();
            }

            if (await _dbContext.Submissions.FindAsync(reviewRequest.SubmissionId) == null )
            {
                throw new BadRequestException($"Foreign Key - Submission Id : {reviewRequest.SubmissionId} Does not Exist");
            }
            if ( await _dbContext.Mentors.FindAsync(reviewRequest.MentorId) == null)
            {
                throw new BadRequestException($"Foreign Key - Mentor Id : {reviewRequest.MentorId} Does not Exist");
            }
            
            Review review = new()
            {
                SubmissionId = reviewRequest.SubmissionId,
                MentorId = reviewRequest.MentorId,
                Feedback = reviewRequest.FeedBack,
                Score = reviewRequest.Score,
                ReviewStatus = reviewRequest.ReviewStatus,
                ReviewedDate = reviewRequest.ReviewedDate
            };

            _dbContext.Reviews.Add(review);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("CreateAsync:Review : New Review created with id {id} ", review.Id);
            return MapToResponse(review);
        }

        public static ReviewResponse MapToResponse(Review review)
        {
            return new ReviewResponse
            {
                Id = review.Id,
                SubmissionId = review.SubmissionId,
                Submission = review.Submission,
                MentorId = review.MentorId,
                Mentor = review.Mentor,
                Feedback = review.Feedback,
                Score = review.Score,
                ReviewStatus = review.ReviewStatus,
                ReviewedDate = review.ReviewedDate
            };
        }

        private bool CheckAuthorization(int payloadMentorId)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            _ = int.TryParse(user?.GetUserId(), out int authenticatedUid);            
            string currentRole = user?.GetRole() ?? string.Empty;

            bool isAdmin = string.Equals(currentRole, nameof(Role.Admin), StringComparison.OrdinalIgnoreCase);
            bool isMentor = string.Equals(currentRole, nameof(Role.Mentor), StringComparison.OrdinalIgnoreCase);

            
            if (isAdmin) return true;

            
            if (isMentor && authenticatedUid != payloadMentorId)
            {
                return false;
            }

            return true;
            
        }

    }
}
