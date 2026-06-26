using Models;
using SubmissionProcessingWorker.DTOs;

namespace SubmissionProcessingWorker.Services
{
    public interface ITrainingDirectoryClient
    {
        public Task<Trainee?> GetTrainee(TraineeRequest traineeRequest,  CancellationToken cancellationToken);
    }
}