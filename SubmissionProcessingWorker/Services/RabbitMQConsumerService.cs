using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks.Dataflow;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Models;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using SubmissionProcessingWorker.DTOs;
using SubmissionProcessingWorker.Utilities;
using Superpower.Parsers;
using TraineeManagement.Shared.Contracts;
using TraineeManagement.Shared.Models;

namespace SubmissionProcessingWorker.Services;  
 
public class RabbitMQConsumerService : BackgroundService  
{  
    private readonly ILogger<RabbitMQConsumerService> _logger;  
    private readonly RabbitMqSettings _options;  
    private readonly FileConfig _fileOptions;  
    private IConnection? _connection;  
    private IChannel? _channel;  
    private string? _queueName;  
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ITrainingDirectoryClient _httpClient ;


    private const int MaxRetries = 3;

    public RabbitMQConsumerService(  
        ILogger<RabbitMQConsumerService> logger,  
        IOptions<RabbitMqSettings> options,
        IOptions<FileConfig> fileOptions,
        IServiceScopeFactory serviceScopeFactory, 
        ITrainingDirectoryClient client)  
    {  
        _logger = logger;  
        _options = options.Value;  
        _fileOptions = fileOptions.Value;
        _serviceScopeFactory = serviceScopeFactory;
        _httpClient = client;
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
            IConnectionFactory factory = _options.CreateConnectionFactory();

            AsyncRetryPolicy connectionRetryPolicy = Policy
                .Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetryForeverAsync(
                    retryAttempt => TimeSpan.FromSeconds(Math.Min(30, Math.Pow(2, retryAttempt))),
                   (exception, timeSpan) =>
                    { 
                        _logger.LogWarning("RabbitMQ offline. Retrying connection in {Delay}s", timeSpan.TotalSeconds);
                    });
        try  
        {  
            //Consumer Retry Policy for Connection
            await connectionRetryPolicy.ExecuteAsync(async () =>
            {
                if (_connection == null)
                {
                    _connection = await factory.CreateConnectionAsync(cancellationToken: stoppingToken);  
                    _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);  
                } 
            });
           
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
                exchange: "dlq-submission-processing-exchange",  
                routingKey: _dlq_queueName,
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
            string CorrelationId = ea.BasicProperties.CorrelationId!;
            _logger.LogInformation("correlationId : {correlationId} Received message: {message}",CorrelationId,message);  
            
            SubmissionProcessingRequested? payload = null;
            try  
            {  
                payload = JsonSerializer.Deserialize<SubmissionProcessingRequested>(message) ?? throw new Exception($"correlationId : {CorrelationId} Payload could not be Desialized to SubmissionProcessingRequested");

                await ProcessMessageAsync(payload, stoppingToken,CorrelationId); 
                
                //Updating Status after processingthe job
                await UpdateStatus(payload,ProcessingJobStatus.Completed,correlationId:CorrelationId, cancellationToken:stoppingToken); 
 
                // Ack success
                await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);  
                _logger.LogInformation("correlationId : {correlationId} Message acknowledged.", CorrelationId);  
            }  
            catch (Exception ex)  
            {  
                _logger.LogError(ex, "correlationId : {correlationId} Failed to process message.",CorrelationId);  

                if (payload != null)
                {
                    using (IServiceScope scope = _serviceScopeFactory.CreateScope())
                    {
                        AppDbContext _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        ProcessingJob? job = await _dbContext.ProcessingJobs.FirstOrDefaultAsync(p => p.CorrelationId == payload.CorrelationId) ?? throw new Exception("ProcessingJob not found");
                        
                        bool shouldRetry = job != null && job.Attempts < MaxRetries;
                        if (!shouldRetry)
                        {
                            _logger.LogError("correlationId : {correlationId} Setting Status as Failed", CorrelationId);  
                            await UpdateStatus(payload,ProcessingJobStatus.Failed,correlationId:CorrelationId ,ErrorMessage: ex.Message.ToString());
                        }
                        
                        await _channel.BasicNackAsync(  
                            deliveryTag: ea.DeliveryTag,  
                            multiple: false,  
                            requeue: shouldRetry,
                            cancellationToken: stoppingToken    
                        );

                        _logger.LogError("correlationId : {correlationId} NACK SENT with requeue as {s}", CorrelationId ,shouldRetry);  

                    }
                } else
                {
                    await _channel.BasicNackAsync(  
                    deliveryTag: ea.DeliveryTag,  
                    multiple: false,  
                    requeue: false,
                    cancellationToken: stoppingToken    
                    );  
                    
                    _logger.LogError("correlationId : {correlationId} NACK SENT with requeue as {s}",CorrelationId ,false);  

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
 
 
    private async Task<SubmissionProcessingRequested> ProcessMessageAsync(SubmissionProcessingRequested payload, CancellationToken stoppingToken, string correlationId)  
    {
        _logger.LogInformation("correlationId : {correlationId} Message processing.", correlationId);  
        await UpdateStatus(payload,ProcessingJobStatus.Processing, correlationId, cancellationToken: stoppingToken);

        //CALCULATING CHECKSUM
        // pay has submissionID then submissionID has checkSum
        using IServiceScope scope = _serviceScopeFactory.CreateScope();
        AppDbContext _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        //File has checkSum value and generatedStorageName
        SubmissionFile? file = await _dbContext.SubmissionFiles.FirstOrDefaultAsync(f => f.Id == payload.FileId, cancellationToken:stoppingToken) ?? throw new Exception("Submission File data not found");
        
        //loading file and then checking checksum
        string root = _fileOptions.Location!;
        string path = Path.Combine(root,file.GeneratedStorageName);
        await using FileStream fileStream = File.OpenRead(path);
        string checksum = GenerateCheckSum(fileStream);

        //Checking Checksum
        if(file.Checksum == checksum)
        {
            _logger.LogInformation("correlationId : {correlationId} File CheckSum Validated", correlationId);
        }
        else
        {
            _logger.LogError("correlationId : {correlationId} FileCheckSum Is Invalid and did not match", correlationId);
            throw new Exception("Your CheckSum is not Valid");
        }
        //EXTRACTING SAFE METADDATA
        _logger.LogInformation("correlationId : {correlationId} File MetaData is : Name : {name} , Length: {length}, Extension: {type}",correlationId ,Path.GetFileName(fileStream.Name), fileStream.Length,Path.GetExtension(fileStream.Name));


        //Interprocess Communication Demo
        _logger.LogInformation("correlationId : {correlationId} Internal Service Calligng via typed Client", correlationId);
        Trainee? result = await _httpClient.GetTrainee(new TraineeRequest(){ SubmissionId = payload.SubmissionId , CorrelationId = correlationId}, stoppingToken);
        _logger.LogInformation("correlationId : {correlationId} Internal Service returned result {result}", correlationId,JsonSerializer.Serialize(result));


        //Testing for DLQ
        // throw new Exception("Something Happened");
          
        await Task.Delay(3000);    
        _logger.LogInformation("correlationId : {correlationId} Message processed successfully.", correlationId);  

        return payload;
    }

    private string GenerateCheckSum(FileStream fileStream)
    {
        using (MD5 md5 = System.Security.Cryptography.MD5.Create())
        {
            
            byte[] hash = md5.ComputeHash(fileStream);
            return BitConverter.ToString(hash).Replace("-", "");
            
        }
    }

    public async Task UpdateStatus( SubmissionProcessingRequested payload,ProcessingJobStatus status, string correlationId , string ErrorMessage = "default", CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("correlationId: {correlationID} Updating status to {status} ", correlationId, status.ToString());
        using (IServiceScope scope = _serviceScopeFactory.CreateScope())
        {
            AppDbContext _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            ProcessingJob? job = await _dbContext.ProcessingJobs.FirstOrDefaultAsync(p => p.CorrelationId == payload.CorrelationId) ?? throw new Exception("Processing JOB not found");
            
            //When Processing 
            if (status == ProcessingJobStatus.Processing)
            {
                if (job.ProcessingJobStatus == ProcessingJobStatus.Completed) {
                    _logger.LogInformation("correlationId: {correlationId} Job already completed.",correlationId);
                    return;
                }

                job.ProcessingJobStatus = status;
                job.Attempts++;
                job.Version++;
            }
            else if (status == ProcessingJobStatus.Completed)
            {
                job.ProcessingJobStatus = status;
                job.CompletedAt = DateTime.UtcNow;
                job.Version++;
            }
            else if (status == ProcessingJobStatus.Failed)
            {
                job.ProcessingJobStatus = status;
                job.ErrorSummary = ErrorMessage;
                job.Version++;
            }
            
            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning(ex,"Concurrency conflict while updating job {CorrelationId}",correlationId);
                throw;
            }
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