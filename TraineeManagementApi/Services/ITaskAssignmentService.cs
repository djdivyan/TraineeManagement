using Models;
using TraineeManagementApi.DTOs;

namespace TraineeManagementApi.Services
{
    public interface ITaskAssignmentService
    {
        Task<List<TaskAssignmentResponse>> GetAllAsync();
        Task<TaskAssignmentResponse> GetByIdAsync(int id, CancellationToken cancellationToken);        
        Task<TaskAssignmentResponse> CreateAsync(TaskAssignmentRequest request);
        Task<TaskAssignmentResponse> UpdateAsync(int id, UpdateTaskAssignmentRequest request, CancellationToken cancellationToken);

        
        // Task<bool> DeleteAsync(int id);
        // Task<PaginationResponse<MentorResponse>> GetPagedDataAsync(PaginationRequest paginationRequest);
    } 
}