using Models;

namespace TrainingDirectory.Api.Services
{
    public interface ITrainingDirectoryService
    {
        Task<Trainee?> GetTraineeAsync(int Id,CancellationToken cancellationToken = default);
    }
}