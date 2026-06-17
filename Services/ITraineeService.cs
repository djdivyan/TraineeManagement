using Models;
using TraineeManagementApi.DTOs;

namespace TraineeManagementApi.Services
{
    public interface ITraineeService
    {
        Task<IEnumerable<TraineeResponse>> GetAllAsync(string? search);
        Task<TraineeResponse> GetByIdAsync(int id);
        Task<TraineeResponse> CreateAsync(CreateTraineeRequest createTraineeRequest);
        Task<TraineeResponse> UpdateAsync(int id,UpdateTraineeRequest updateTraineeRequest);
        Task<bool> DeleteAsync(int id);
        Task<PaginationResponse<TraineeResponse>> GetPagedDataAsync(PaginationRequest paginationRequest);
    } 
}