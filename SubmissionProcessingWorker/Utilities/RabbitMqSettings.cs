namespace SubmissionProcessingWorker.Utilities
{
    public class RabbitMqSettings : BaseRabbitMqSettings
    {
        public string QueueName { get; set; }
        public string ExchangeName {get; set;}
    }
}