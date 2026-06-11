using System.Text.Json.Serialization;
using TraineeManagementApi.Services;
using Microsoft.EntityFrameworkCore;
using TraineeManagementApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();

// builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("TraineeList"));
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));


//TO use json converter enum to string in all req res
// .AddJsonOptions(options =>
// {
//     options.JsonSerializerOptions.Converters.Add(
//         new JsonStringEnumConverter()
//     );
// });


// builder.Services.AddValidation();

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
// builder.Services.AddSwaggerGen();

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



builder.Services.AddScoped<ITraineeService, TraineeService>();
builder.Services.AddScoped<IUserSevice, UserSevice>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger(); // Serves the Swagger JSON
    app.UseSwaggerUI(); // Serves Swagger UI
}

app.UseHttpsRedirection();

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
