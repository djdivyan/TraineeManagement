using Microsoft.EntityFrameworkCore;
using Models;
using TraineeManagement.Shared.Models;
using TrainingDirectory.Api.Services;

namespace TraineeDirectory.Api.Services
{
    public class TrainingDirectoryService(AppDbContext appDbContext) : ITrainingDirectoryService
    {   
        private readonly AppDbContext _dbContext = appDbContext;
        public async Task<Trainee?> GetTraineeAsync(int Id, CancellationToken cancellationToken = default)
        {
            
            return await _dbContext.Submissions.Where(s => s.Id == Id).Select(s => s.TaskAssignment.Trainee).FirstOrDefaultAsync();
        }
    }
}