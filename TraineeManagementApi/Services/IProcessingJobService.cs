using Microsoft.AspNetCore.Mvc;
using Models;
using TraineeManagementApi.DTOs;

namespace TraineeManagementApi.Services
{
    public interface IProcessingJobService
    {
        Task<ProcessingJob> GetJobByIdAsync(int id);
        Task<ProcessingJob> RetryJob(Guid id, CancellationToken cancellationToken=default);
    } 
}