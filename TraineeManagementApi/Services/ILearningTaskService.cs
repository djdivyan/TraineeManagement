using Models;
using TraineeManagementApi.DTOs;

namespace TraineeManagementApi.Services
{
    public interface ILearningTaskService
    {
        Task<List<LearningTaskResponse>> GetAllAsync(string? search);
        Task<LearningTaskResponse?> GetByIdAsync(int id);
        
        Task<LearningTaskResponse> CreateAsync(LearningTaskRequest learningTaskRequest);
        Task<LearningTaskResponse?> UpdateAsync(int id, UpdateLearningTaskRequest updateLearningTaskRequest);
        Task<bool> DeleteAsync(int id);
        // Task<PaginationResponse<MentorResponse>> GetPagedDataAsync(PaginationRequest paginationRequest);
    } 
}