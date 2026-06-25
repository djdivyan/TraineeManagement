using Microsoft.EntityFrameworkCore;
using Models;
using TraineeManagementApi.DTOs;
using TraineeManagementApi.Exceptions;
using TraineeManagement.Shared.Models;

namespace TraineeManagementApi.Services
{
    class ProcessingJobService(AppDbContext dbContext, ILogger<ProcessingJobService> logger) : IProcessingJobService
    {
        private readonly AppDbContext _dbContext = dbContext;
        private readonly ILogger<ProcessingJobService> _logger = logger;

        public async Task<ProcessingJob> GetJobByIdAsync(int id)
        {
            _logger.LogInformation("GetByIdAsync:ProcessingJob : Entering the Function");            

            ProcessingJob? processingJob = await _dbContext.ProcessingJobs.FindAsync(id);
            if (processingJob is null)
            {
                _logger.LogError("GetByIdAsync:ProcessingJob : ProcessingJob Not found with {id}", id);
                throw new NotFoundException("Review",id);
            }
            _logger.LogInformation("GetByIdAsync:ProcessingJob : ProcessingJob found with {id}", id);
            return processingJob;
        }
    }
}
