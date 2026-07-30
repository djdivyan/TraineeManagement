using Microsoft.AspNetCore.Mvc;
using Models;
using TraineeManagementApi.DTOs;

namespace TraineeManagementApi.Services
{
    public interface ISubmissionService
    {
        Task<List<SubmissionResponse>> GetAllAsync();
        Task<SubmissionResponse> GetByIdAsync(int id);        
        Task<SubmissionResponse> CreateAsync(SubmissionRequest request);
        Task<SubmissionFileResponseDTO> SaveFileAsync(int submissionId,SubmissionFileRequestDTO request, CancellationToken cancellationToken);
        Task<SubmissionSummaryDTO> GetSubmissionSummaryAsync(int submissionid, CancellationToken cancellationToken);
        Task<Submission?> GetRawByIdAsync(int id);
    } 
}