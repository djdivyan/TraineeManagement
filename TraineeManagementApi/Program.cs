using System.Text.Json.Serialization;
using TraineeManagementApi.Services;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi;
using TraineeManagementApi.DTOs;
using Microsoft.JSInterop.Infrastructure;
using DotNetEnv;
using Microsoft.Extensions.FileProviders;
using TraineeManagementApi.Utilities;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using RabbitMQ.Client;

Env.Load();

var  MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
 
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy  =>
                      {
                          policy.WithOrigins("http://localhost:3000",
                                              "http://localhost:5173");
                      });
});


builder.Logging.ClearProviders();
builder.Logging.AddConsole();


builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(
        new JsonStringEnumConverter()
    );
});;

builder.Configuration.AddEnvironmentVariables();
// builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("TraineeList"));
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddAuthentication(options =>
{
   options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
   options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
   options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
   options.RequireHttpsMetadata = false;
   options.SaveToken = true;
   options.TokenValidationParameters = new TokenValidationParameters
   {
      ValidIssuer = builder.Configuration["JwtConfig:Issuer"],
      ValidAudience = builder.Configuration["JwtConfig:Audience"],
      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtConfig:Key"]!)),
      ValidateIssuer = true,
      ValidateAudience = true,
      ValidateLifetime = true,
      ValidateIssuerSigningKey = true
   };
});
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TraineeManagement.Api",
        Version = "v1"
    });
 
    // Define security scheme
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Type = SecuritySchemeType.ApiKey,
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "Enter your JWT token status below."
 
    });
 
    // Apply security to endpoints
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

//RabbitMQ settings
builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMq"));


builder.Services.AddScoped<ITraineeService, TraineeService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IMentorService, MentorService>();
builder.Services.AddScoped<ILearningTaskService, LearningTaskService>();
builder.Services.AddScoped<ITaskAssignmentService, TaskAssignmentService>();
builder.Services.AddScoped<ISubmissionService, SubmissionService>();
builder.Services.AddScoped<IReviewService, ReviewService>();

builder.Services.AddScoped<IFileStorageService, LocalFileManagerService>();
builder.Services.AddScoped<ISubmissionFileService, SubmissionFileService>();
builder.Services.AddScoped<IProcessingJobService, ProcessingJobService>();

builder.Services.AddScoped<ICacheService, CacheService>();

builder.Services.AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>();


//Redis Cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "TraineeManagementAPI:";
});

//Exception
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();


builder.Services.AddHealthChecks()
    .AddMySql(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "mysql",
        failureStatus: HealthStatus.Unhealthy,
        timeout: TimeSpan.FromSeconds(5),
        tags: new[] { "ready", "mysql" })
    .AddRedis(
        redisConnectionString: builder.Configuration.GetConnectionString("Redis")!,
        name: "redis",
        failureStatus: HealthStatus.Unhealthy,
        timeout: TimeSpan.FromSeconds(5),
        tags: new[] { "ready" , "redis"})
    .AddRabbitMQ(
        async sp =>
        {
            var factory = new ConnectionFactory
            {
               HostName = builder.Configuration["RabbitMq:HostName"]!,
               UserName = builder.Configuration["RabbitMq:UserName"]!,
               Password = builder.Configuration["RabbitMq:Password"]!,
               Port = int.TryParse(builder.Configuration["RabbitMq:Port"], out var port) ? port : 5672,
               VirtualHost = builder.Configuration["RabbitMq:VirtualHost"]!
            };
            return await factory.CreateConnectionAsync();
        },
        name: "rabbitmq",
        failureStatus: HealthStatus.Unhealthy,
        timeout: TimeSpan.FromSeconds(5),
        tags: new[] { "ready" , "rabbitmq"})
    .AddUrlGroup(
        uri: new Uri(builder.Configuration["InternalService:Url"]!),
        name: "TraineeDirectory.Api",
        failureStatus: HealthStatus.Unhealthy,
        timeout: TimeSpan.FromSeconds(5),
        tags: new[] { "ready", "TraineeDirectory.Api" });

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "live" });




var app = builder.Build();

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live"),
    ResponseWriter = async(context,report) =>
    {
        context.Response.ContentType = "application/json";

        var result = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description
            })
        };

        await context.Response.WriteAsJsonAsync(result);
    }
});

// Readiness endpoint — checks dependencies
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = async(context,report) =>
    {
        context.Response.ContentType = "application/json";

        var result = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description
            })
        };

        await context.Response.WriteAsJsonAsync(result);
    }
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger(); // Serves the Swagger JSON
    app.UseSwaggerUI(options =>
    {
        options.EnablePersistAuthorization();
    }); // Serves Swagger UI
}

app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseHttpsRedirection();

// app.UseStaticFiles(new StaticFileOptions
// {
//     FileProvider = new PhysicalFileProvider(
//            Path.Combine(builder.Environment.ContentRootPath, "Uploads")),
//     RequestPath = "/Resources"
// });


app.UseCors(MyAllowSpecificOrigins);


app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

using(var scope = app.Services.CreateAsyncScope())
{
   var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
 
   if (!db.Users.Any())
   {
      var admin = new User
      {
         Username = "admin",
         Email = "admin@gmail.com",
         Role = Role.Admin,
         CreatedDate = DateTime.Now
      };
      var hasher = new PasswordHasher<User>();
      string hashedPassword = hasher.HashPassword(admin, "admin");
      admin.PasswordHash = hashedPassword;
      Console.WriteLine("Seeding user: " + admin);
      db.Users.Add(admin);
      db.SaveChanges();
   }
}

app.Run();
