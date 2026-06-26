using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using TraineeDirectory.Api.Services;
using TraineeManagement.Shared.Models;
using TrainingDirectory.Api.Services;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddScoped<ITrainingDirectoryService, TrainingDirectoryService>();


builder.Configuration.AddEnvironmentVariables();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

builder.Services.AddControllers();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();



app.UseHttpsRedirection();



app.Run();
