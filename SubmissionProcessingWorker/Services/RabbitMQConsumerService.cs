using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks.Dataflow;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using SubmissionProcessingWorker.Utilities;
using TraineeManagementApi.Contracts;
using TraineeManagementApi.Exceptions;
using TraineeManagementApi.Models;
using TraineeManagementApi.Services;

namespace SubmissionProcessingWorker.Services;  
 
public class RabbitMQConsumerService : BackgroundService  
{  
    private readonly ILogger<RabbitMQConsumerService> _logger;  
    private readonly RabbitMqSettings _options;  
    private IConnection? _connection;  
    private IChannel? _channel;  
    private string? _queueName;  
    private readonly IServiceScopeFactory _serviceScopeFactory;

    private const int MaxRetries = 3;

    public RabbitMQConsumerService(  
        ILogger<RabbitMQConsumerService> logger,  
        IOptions<RabbitMqSettings> options,
        IServiceScopeFactory serviceScopeFactory)  
    {  
        _logger = logger;  
        _options = options.Value;  
        _serviceScopeFactory = serviceScopeFactory;
    }  


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)  
    {  
        
        stoppingToken.Register(() =>  
            _logger.LogInformation("Consumer service is stopping."));  
 
        await InitializeRabbitMQ(stoppingToken);  
 
        await ConsumeMessagesAsync(stoppingToken);  
    }  
 
    private async Task InitializeRabbitMQ(CancellationToken stoppingToken)  
    {  
        try  
        {  
            IConnectionFactory factory = _options.CreateConnectionFactory();
 
            if (_connection == null)
            {
                _connection = await factory.CreateConnectionAsync(cancellationToken: stoppingToken);  
                _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);  
            } 
            _logger.LogInformation("Connected to RabbitMQ server.");  
 
             if (_channel == null) throw new InvalidOperationException("Channel is not initialized.");

            //add QUEUE FOR DEAD LETTER WITH EXCHANGE,QUUEU
            await _channel.ExchangeDeclareAsync(
                exchange: "dlq-submission-processing-exchange",
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false, 
                cancellationToken: stoppingToken);
            
            string _dlq_queueName = await _channel.QueueDeclareAsync(  
                queue: "dlq-submission-processing",  
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: stoppingToken
            );  
 
            // Bind queue to exchange with routing key  
            await _channel.QueueBindAsync(  
                queue: _dlq_queueName,  
                exchange: _options.ExchangeName,  
                routingKey: _options.QueueName,
                arguments: null,
                cancellationToken: stoppingToken
            );  



            await _channel.ExchangeDeclareAsync(  
                exchange: _options.ExchangeName,  
                type: ExchangeType.Direct,  
                durable: true,    
                autoDelete: false,
                cancellationToken : stoppingToken
            );  
 
            Dictionary<string,object?> queueArgs = new Dictionary<string, object?>  
            {  
                {"x-dead-letter-exchange", "dlq-submission-processing-exchange"},  
                {"x-dead-letter-routing-key", "dlq-submission-processing"}  
            };  

            _queueName = await _channel.QueueDeclareAsync(  
                queue: _options.QueueName,  
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: queueArgs,
                cancellationToken: stoppingToken
            );  
 
            // Bind queue to exchange with routing key  
            await _channel.QueueBindAsync(  
                queue: _queueName,  
                exchange: _options.ExchangeName,  
                routingKey: _options.QueueName,
                arguments: null,
                cancellationToken: stoppingToken
            );  


            await _channel.BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: 1,
                global: false,
                cancellationToken: stoppingToken
            );
 
            _logger.LogInformation($"Queue '{_queueName}' bound to exchange '{_options.ExchangeName}'.");  
        }  
        catch (BrokerUnreachableException ex)  
        {  
            _logger.LogCritical(ex, "Failed to connect to RabbitMQ. Check if the server is running.");  
            throw;
        }  
        catch (Exception ex)  
        {   
            _logger.LogWarning("There is a  error, Configuration Data :  {settings}",JsonSerializer.Serialize(_options));  
            _logger.LogError(ex, "Error initializing RabbitMQ.");  
            throw;  
        }  
    }  
 
    private async Task ConsumeMessagesAsync(CancellationToken stoppingToken)  
    {  
        if (_channel == null || string.IsNullOrEmpty(_queueName))  
        {  
            _logger.LogError("Channel or queue not initialized.");  
            return;  
        }  
 
        AsyncEventingBasicConsumer consumer = new AsyncEventingBasicConsumer(_channel);  
        
        consumer.ReceivedAsync += async (ch, ea) =>  
        {  
            byte[] body = ea.Body.ToArray();  
            string message = Encoding.UTF8.GetString(body);  
 
            _logger.LogInformation("Received message: {message}",message);  
            
            SubmissionProcessingRequested? payload = null;
            try  
            {  
                
                payload = JsonSerializer.Deserialize<SubmissionProcessingRequested>(message) ?? throw new Exception("Payload could not be Desialized to SubmissionProcessingRequested");

                await ProcessMessageAsync(message); 
                
                //Updating Status after processingthe job
                await UpdateStatus(payload,ProcessingJobStatus.Completed); 
 
                // Ack success
                await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);  
                _logger.LogInformation("Message acknowledged.");  
            }  
            catch (Exception ex)  
            {  
                _logger.LogError(ex, "Failed to process message.");  

                if (payload != null)
                {
                    using (IServiceScope scope = _serviceScopeFactory.CreateScope())
                    {
                        AppDbContext _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        ProcessingJob? job = await _dbContext.ProcessingJobs.FirstOrDefaultAsync(p => p.CorrelationId == payload.CorrelationId) ?? throw new Exception("ProcessingJob not found");
                        
                        bool shouldRetry = job != null && job.Attempts < MaxRetries;
                        if (!shouldRetry)
                        {
                            _logger.LogError("Setting Status as Failed");  
                            await UpdateStatus(payload,ProcessingJobStatus.Failed, ex.Message.ToString());
                        }
                        
                        await _channel.BasicNackAsync(  
                            deliveryTag: ea.DeliveryTag,  
                            multiple: false,  
                            requeue: shouldRetry,
                            cancellationToken: stoppingToken    
                        );

                        _logger.LogError("NACK SENT {s}",shouldRetry);  

                    }
                } else
                {
                    await _channel.BasicNackAsync(  
                    deliveryTag: ea.DeliveryTag,  
                    multiple: false,  
                    requeue: false,
                    cancellationToken: stoppingToken    
                    );  
                    
                    _logger.LogError("NACK SENT {s}",false);  

                }
            }  
        };  
 
        // Start consuming  
        string consumerTag = await _channel.BasicConsumeAsync(  
            queue: _queueName,  
            autoAck: false, 
            consumer: consumer  
        );  
 
        _logger.LogInformation("Consumer started. Waiting for messages...");  
 
        // Keep the service running until stopped  
        await Task.Delay(Timeout.Infinite, stoppingToken);  
    }  
 
    // Example message processing logic  
    private async Task<SubmissionProcessingRequested> ProcessMessageAsync(string message)  
    {
        SubmissionProcessingRequested? payload = JsonSerializer.Deserialize<SubmissionProcessingRequested>(message) ?? throw new Exception("Payload could not be Desialized to SubmissionProcessingRequested");
        _logger.LogInformation("Message processing.");  
        await UpdateStatus(payload,ProcessingJobStatus.Processing);

        //CALCULATING CHECKSUM
        // pay has submissionID then submissionID has checkSum
        // Read the file via openReadAsync
        
        //
        using IServiceScope scope = _serviceScopeFactory.CreateScope();
        AppDbContext _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        //File has checkSum value and generatedStorageName
        SubmissionFile? file = await _dbContext.SubmissionFiles.FirstOrDefaultAsync(f => f.Id == payload.FileId) ?? throw new Exception("Submission File data not found");
        

        IFileStorageService service = scope.ServiceProvider.GetRequiredService<IFileStorageService>();
        //Pass File Name with Extension then call generate checksum
        FileStream fileStream = await service.OpenReadAsync(file.GeneratedStorageName);
        //Checking Checksum
        if(file.Checksum == GenerateCheckSum(fileStream))
        {
            _logger.LogInformation("CheckSum Validated");
        }
        else
        {
            _logger.LogError("FileCheckSum Did not match");
            throw new 
        }

        using (MD5 md5 = System.Security.Cryptography.MD5.Create())
        {
            
            byte[] hash = md5.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "");
            
        }

        //EXTRACTING SAFE METADDATA
        //PRODUCING A GENERATED RESULT FILE 
        await Task.Delay(3000);    



        //Testing for DLQ
        // throw new Exception("Something Happened");
        
        // Simulate work (e.g., save to DB, call API)  
        _logger.LogInformation("Message processed successfully.");  

        return payload;
    }


    public async Task UpdateStatus( SubmissionProcessingRequested payload,ProcessingJobStatus status, string ErrorMessage = "default", CancellationToken cancellationToken = default)
    {
        using (IServiceScope scope = _serviceScopeFactory.CreateScope())
        {
            Guid CorrelationId =  payload.CorrelationId;
            AppDbContext _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            ProcessingJob? job = await _dbContext.ProcessingJobs.FirstOrDefaultAsync(p => p.CorrelationId == payload.CorrelationId) ?? throw new Exception("Processing JOB not found");
            
            //When Processing 
            if (status == ProcessingJobStatus.Processing)
            {
                if (job.ProcessingJobStatus == ProcessingJobStatus.Completed) {
                    throw new InvalidOperationException($"Job Process with Correlation Id Already Processed {CorrelationId}");
                }

                job.ProcessingJobStatus = status;
                job.Attempts++;
            }
            if (status == ProcessingJobStatus.Completed)
            {
                job.ProcessingJobStatus = status;
                job.CompletedAt = DateTime.UtcNow;
            }
            if (status == ProcessingJobStatus.Failed)
            {
                job.ProcessingJobStatus = status;
                job.ErrorSummary = ErrorMessage;
            }
            
            await _dbContext.SaveChangesAsync(cancellationToken);
        };
    }  
 
    // Cleanup resources when the service stops

    
    public override void Dispose()  
    {  
        _channel?.CloseAsync();  
        _channel?.Dispose();  
        _connection?.CloseAsync();  
        _connection?.Dispose();  
 
        base.Dispose();  
        _logger.LogInformation("Consumer service disposed.");  
    }  
}  