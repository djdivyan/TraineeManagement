using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using SubmissionProcessingWorker;
using SubmissionProcessingWorker.Services;
using SubmissionProcessingWorker.Utilities;
using TraineeManagement.Shared.Models;

Env.Load();

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMq"));

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));



builder.Services.AddHostedService<RabbitMQConsumerService>();
// builder.Services.AddHostedService<Worker>();


var host = builder.Build();
host.Run();
