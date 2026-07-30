using System.Net;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Extensions.Http;
using SubmissionProcessingWorker;
using System.Net.Http;

using SubmissionProcessingWorker.Services;
using SubmissionProcessingWorker.Utilities;
using TraineeManagement.Shared.Models;
using RabbitMQ.Client.Exceptions;

Env.Load();

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMq"));
builder.Services.Configure<FileConfig>(builder.Configuration.GetSection("File"));

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));


//IhttpClient Configuration
builder.Services.AddHttpClient<ITrainingDirectoryClient, TrainingDirectoryClient>( client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["InternalService:Url"]!);
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        client.DefaultRequestHeaders.UserAgent.ParseAdd("SubmissoinProcessingWorker");
        client.DefaultRequestHeaders.Add("X-API-KEY", builder.Configuration["InternalService:ApiKey"]);
        client.Timeout = TimeSpan.FromSeconds(20);

    }).ConfigurePrimaryHttpMessageHandler(() =>
        {
            return new SocketsHttpHandler()
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(15)
            };
        })
        .SetHandlerLifetime(Timeout.InfiniteTimeSpan)
        .AddStandardResilienceHandler(options =>
        {
            options.Retry.MaxRetryAttempts = 5;
            options.Retry.BackoffType = DelayBackoffType.Exponential;
            options.Retry.UseJitter = true;
        
            options.Retry.ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                .Handle<HttpRequestException>()
                .HandleResult(response =>
                response.StatusCode == HttpStatusCode.RequestTimeout ||
                response.StatusCode == HttpStatusCode.ServiceUnavailable ||
                response.StatusCode == HttpStatusCode.TooManyRequests ||
                (int)response.StatusCode >= 500
                );
            
        
            options.CircuitBreaker.FailureRatio = 0.5;
            options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(20);
            options.CircuitBreaker.MinimumThroughput = 5;
            options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(30);
        
            options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);
        });

builder.Services.AddHostedService<RabbitMQConsumerService>();
// builder.Services.AddHostedService<Worker>();




var host = builder.Build();
host.Run();
