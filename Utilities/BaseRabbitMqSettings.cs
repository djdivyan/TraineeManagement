using RabbitMQ.Client;

namespace TraineeManagementApi.Utilities
{
    public class BaseRabbitMqSettings
    {
        public string HostName { get; set; } = string.Empty;
        public string VirtualHost { get; set; } = string.Empty;
        public int Port { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
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