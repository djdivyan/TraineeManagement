using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using TraineeManagementApi.Contracts;

namespace TraineeManagementApi.Services
{
    public interface IRabbitMqPublisher
    {
        Task PublishAsync(string queueName, SubmissionProcessingRequested message, CancellationToken cancellationToken = default);
    }
}
