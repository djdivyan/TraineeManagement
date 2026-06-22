using Models;
using TraineeManagementApi.DTOs;

namespace TraineeManagementApi.Services
{
    public interface ITraineeService
    {
        Task<IEnumerable<TraineeResponse>> GetAllAsync(string? search);
        Task<TraineeResponse> GetByIdAsync(int id,CancellationToken cancellationToken);
        Task<TraineeResponse> CreateAsync(CreateTraineeRequest createTraineeRequest);
        Task<TraineeResponse> UpdateAsync(int id,UpdateTraineeRequest updateTraineeRequest, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
        Task<PaginationResponse<TraineeResponse>> GetPagedDataAsync(PaginationRequest paginationRequest);
    } 
}