using Models;
using TraineeManagementApi.DTOs;

namespace TraineeManagementApi.Services
{
    public interface IReviewService
    {
        Task<List<ReviewResponse>> GetAllAsync();
        Task<ReviewResponse> GetByIdAsync(int id);        
        Task<ReviewResponse> CreateAsync(ReviewRequest request);
    
    } 
}