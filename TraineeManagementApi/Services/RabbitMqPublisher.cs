using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using TraineeManagementApi.Utilities;
using TraineeManagement.Shared.Contracts;
using System.Runtime.CompilerServices;

namespace TraineeManagementApi.Services
{   
    public class RabbitMqPublisher : IRabbitMqPublisher, IAsyncDisposable
    {
        private readonly IConnectionFactory _factory;
        private IConnection? _connection;
        private IChannel? _channel;
        private readonly ILogger<RabbitMqPublisher> _logger;

        public RabbitMqPublisher(IOptions<RabbitMqSettings> settings, ILogger<RabbitMqPublisher> logger)
        {
            _logger =logger;
            _factory = settings.Value.CreateConnectionFactory();
        }

        private async Task InitializeAsync(CancellationToken cancellationToken)
        {
            if (_connection == null)
            {
                _connection = await _factory.CreateConnectionAsync(cancellationToken);
                _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
            }
        }

        public async Task PublishAsync(string queueName, SubmissionProcessingRequested message, CancellationToken cancellationToken = default)
        {
            //Exchange name Keeping as QueName only 
            string exchangeName = queueName;
            await InitializeAsync(cancellationToken);

            if (_channel == null) throw new InvalidOperationException("Channel is not initialized.");
            Dictionary<string,object?> queueArgs = new Dictionary<string, object?>  
            {  
                {"x-dead-letter-exchange", "dlq-submission-processing-exchange"},  
                {"x-dead-letter-routing-key", "dlq-submission-processing"}  
            };  

            await _channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Direct, durable: true, autoDelete: false, cancellationToken: cancellationToken);
            await _channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: queueArgs,
                cancellationToken: cancellationToken
            );
            await _channel.QueueBindAsync(queueName, exchangeName, routingKey: queueName, null, cancellationToken: cancellationToken);

            BasicProperties properties = new BasicProperties
            {
                Persistent = true,
                MessageId = message.MessageId.ToString(),
                CorrelationId = message.CorrelationId.ToString(),
                ContentType="application/json",
                Type = nameof(SubmissionProcessingRequested),
                DeliveryMode = DeliveryModes.Persistent
                
            };

            string json = JsonSerializer.Serialize(message);
            byte[] body = Encoding.UTF8.GetBytes(json);

            try
            {
                await _channel.BasicPublishAsync(
                exchange: exchangeName,
                routingKey: queueName,
                mandatory: true,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken
                );
                _logger.LogInformation("correlationId : {correlationID} Published Message {messageId}",message.CorrelationId,message.MessageId);

            }
            catch (System.Exception ex)
            {
                _logger.LogError("correlationId : {correlationID} Failed to publish message {messageId}",message.CorrelationId,message.MessageId);
                throw;
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel != null) await _channel.CloseAsync();
            if (_connection != null) await _connection.CloseAsync();
        }
    }

}