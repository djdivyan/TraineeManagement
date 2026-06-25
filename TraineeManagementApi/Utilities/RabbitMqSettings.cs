namespace TraineeManagementApi.Utilities
{
    public class RabbitMqSettings : BaseRabbitMqSettings
    {
        public string QueueName { get; set; } = string.Empty;
    }
}