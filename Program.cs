using System.Text.Json.Serialization;
using TraineeManagementApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi();
builder.Services.AddControllers();

//TO use json converter enum to string in all req res
// .AddJsonOptions(options =>
// {
//     options.JsonSerializerOptions.Converters.Add(
//         new JsonStringEnumConverter()
//     );
// });


// builder.Services.AddValidation();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ITraineeService, TraineeService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi();

    app.UseSwagger(); // Serves the Swagger JSON
    app.UseSwaggerUI(); // Serves Swagger UI
}

app.UseHttpsRedirection();

app.UseAuthorization();


app.MapControllers();

app.Run();
