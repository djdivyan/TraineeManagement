using RabbitMQ.Client;

namespace SubmissionProcessingWorker.Utilities
{
    public class BaseRabbitMqSettings
    {
        public string HostName { get; set; }
        public string VirtualHost { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public virtual IConnectionFactory CreateConnectionFactory()
        {
            ConnectionFactory connectionFactory = new ConnectionFactory
            {
                HostName = this.HostName,
                Port = this.Port,
                UserName = this.UserName,
                Password = this.Password,
                VirtualHost = this.VirtualHost,
                AutomaticRecoveryEnabled = true
            };

            return connectionFactory;
        }
    }
}