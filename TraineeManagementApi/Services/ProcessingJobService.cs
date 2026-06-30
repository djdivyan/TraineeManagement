using Microsoft.EntityFrameworkCore;
using Models;
using TraineeManagementApi.DTOs;
using TraineeManagementApi.Exceptions;
using TraineeManagement.Shared.Models;
using TraineeManagement.Shared.Contracts;
using System.Diagnostics;
using System.Threading.Tasks.Dataflow;

namespace TraineeManagementApi.Services
{
    class ProcessingJobService(AppDbContext dbContext, ILogger<ProcessingJobService> logger,IRabbitMqPublisher rabbitMqPublisher) : IProcessingJobService
    {
        private readonly AppDbContext _dbContext = dbContext;
        private readonly ILogger<ProcessingJobService> _logger = logger;
        private readonly IRabbitMqPublisher _publisher = rabbitMqPublisher;
        private const string QueName = "submission-processing";

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

        public async Task<ProcessingJob> RetryJob(Guid id, CancellationToken cancellationToken=default)
        {
            ProcessingJob? job = await _dbContext.ProcessingJobs.FirstOrDefaultAsync(s => s.CorrelationId == id);;
            if (job == null)
            {
                //Unknown Job retry is requested
                throw new NotFoundException("Processing Job",id);
            }

            //if job status is not queued then set status to completed and reset attempt to 0
            if(job.ProcessingJobStatus != ProcessingJobStatus.Queued)
                job.ProcessingJobStatus = ProcessingJobStatus.Queued;

            //Set attempts to 0 since we have to retry
            job.Attempts = 0;
            await _dbContext.SaveChangesAsync();
            
            SubmissionProcessingRequested? message = await _dbContext.SubmissionProcessingRequestedFallback.FirstOrDefaultAsync(s => s.CorrelationId == id);
            if (message == null)
            {   
                //Executes when fallback was not executed                
               //Create a new SubmissionProcessingRequested message to send in queue
                message = new()
                {
                    MessageId = Guid.NewGuid(),
                    SubmissionId = job.SubmissionId,
                    CorrelationId = job.CorrelationId,
                    FileId = job.FileId,
                    RequestedAt = DateTime.UtcNow
                };
            }

            //Publish Message to the queue
            try
            {
                _logger.LogInformation("Publish Job message to the queue");
                await _publisher.PublishAsync(QueName, message, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("RabbitMQ Publish failed Storing job in DB");
                throw new Exception("RabbitMQ could not publish the message"); 
            }

            return job;
        }
    }
}
