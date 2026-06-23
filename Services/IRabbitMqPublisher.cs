using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using TrianeeManagementApi.DTOs;

namespace TraineeManagementApi.Services
{
    public interface IRabbitMqPublisher
    {
        Task PublishAsync(string queueName, SubmissionProcessingRequested message);
    }
}
