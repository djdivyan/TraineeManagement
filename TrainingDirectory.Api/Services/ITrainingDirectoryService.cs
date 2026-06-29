using Models;

namespace TrainingDirectory.Api.Services
{
    public interface ITrainingDirectoryService
    {
        Task<Trainee?> GetTraineeAsync(int Id,string correlationId,CancellationToken cancellationToken = default);
    }
}