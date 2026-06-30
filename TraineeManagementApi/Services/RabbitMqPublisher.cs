using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using TraineeManagementApi.Utilities;
using TraineeManagement.Shared.Contracts;
using System.Runtime.CompilerServices;
using Polly.Retry;
using Polly;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;

namespace TraineeManagementApi.Services
{   
    public class RabbitMqPublisher : IRabbitMqPublisher, IAsyncDisposable
    {
        private readonly IConnectionFactory _factory;
        private IConnection? _connection;
        private IChannel? _channel;
        private readonly ILogger<RabbitMqPublisher> _logger;

        private readonly AsyncRetryPolicy _retryPolicy;

        public RabbitMqPublisher(IOptions<RabbitMqSettings> settings, ILogger<RabbitMqPublisher> logger)
        {
            _logger =logger;
            _factory = settings.Value.CreateConnectionFactory();
            
            _retryPolicy = Policy
            .Handle<BrokerUnreachableException>()
            .Or<AlreadyClosedException>()
            .Or<SocketException>()
            .Or<IOException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,retryAttempt)),
                onRetry: (exception, delay, retryCount, context) =>
                {
                    _logger.LogInformation(exception, "RabbitMQ publish failed. Retry {retry}/3 after {delay}", retryCount,delay.TotalSeconds);
                });
        }

        private async Task InitializeAsync(CancellationToken cancellationToken)
        {
            
            if (_connection?.IsOpen == true && _channel?.IsOpen == true)
            {
                return;
            }

            await _retryPolicy.ExecuteAsync(async () =>
            {
                _connection = await _factory.CreateConnectionAsync(cancellationToken);
                _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
            });
        }

        public async Task PublishAsync(string queueName, SubmissionProcessingRequested message, CancellationToken cancellationToken = default)
        {
            //Exchange name Keeping as QueName only 
            string exchangeName = queueName;
            try
            {
                await InitializeAsync(cancellationToken);            
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "correlationId: {correlatoinId} Failed to Initialize RabbitMQ",message.CorrelationId.ToString());
                throw;
            }

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
                await _retryPolicy.ExecuteAsync(async () =>
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
                });

            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex,"correlationId : {correlationID} Failed to publish message {messageId}",message.CorrelationId,message.MessageId);
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