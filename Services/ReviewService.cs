using Microsoft.EntityFrameworkCore;
using Models;
using TraineeManagementApi.DTOs;
using TraineeManagementApi.Exceptions;
using TraineeManagementApi.Models;

namespace TraineeManagementApi.Services
{
    class ReviewService(AppDbContext dbContext, ILogger<ReviewService> logger) : IReviewService
    {
        private readonly AppDbContext _dbContext = dbContext;
        private readonly ILogger<ReviewService> _logger = logger;


        public async Task<List<ReviewResponse>> GetAllAsync()
        {
            IQueryable<Review> query = _dbContext.Reviews.AsNoTracking();
                                                        // .Include(e => e.Submission)
                                                        // .Include(e => e.Mentor);

            List<Review> reviews = await query.ToListAsync();
            _logger.LogInformation("GetAllAsnc Successfully returned all Reviews");
            return reviews.Select(MapToResponse).ToList();
        }

        public async Task<ReviewResponse> GetByIdAsync(int id)
        {
            Review? review = await _dbContext.Reviews.FindAsync(id);
            if (review is null)
            {
                _logger.LogError("GetByID : Review Not found with {id}", id);
                throw new NotFoundException("Review",id);
            }
            _logger.LogInformation("GetByID : Review found with {id}", id);
            return MapToResponse(review);
        }

        public async Task<ReviewResponse> CreateAsync(ReviewRequest reviewRequest)
        {
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

            _logger.LogInformation("Create : New Review created with id {id} ", review.Id);
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
    }
}
