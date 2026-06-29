using Microsoft.EntityFrameworkCore;
using Models;
using TraineeManagement.Shared.Models;
using TrainingDirectory.Api.Services;

namespace TraineeDirectory.Api.Services
{
    public class TrainingDirectoryService(AppDbContext appDbContext,ILogger<TrainingDirectoryService> logger) : ITrainingDirectoryService
    {   
        private readonly AppDbContext _dbContext = appDbContext;
        private readonly ILogger<TrainingDirectoryService> _logger = logger;
        public async Task<Trainee?> GetTraineeAsync(int Id, string correlationId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("correlationId: {correlationId} GetTraineeAsync: Retrieving Trainee from Training Directory with submission Id {id} ",correlationId, Id);
            return await _dbContext.Submissions.Where(s => s.Id == Id).Select(s => s.TaskAssignment.Trainee).FirstOrDefaultAsync();
        }
    }
}