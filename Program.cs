using System.Text.Json.Serialization;
using TraineeManagementApi.Services;
using Microsoft.EntityFrameworkCore;
using TraineeManagementApi.Models;
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
// var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");
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

var app = builder.Build();

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
