using System.Net.Http.Json;
using System.Text.Json;
using Models;
using RabbitMQ.Client.Exceptions;
using SubmissionProcessingWorker.DTOs;

namespace SubmissionProcessingWorker.Services
{
    public class TrainingDirectoryClient(HttpClient httpClient,ILogger<TrainingDirectoryClient> logger) : ITrainingDirectoryClient
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly ILogger<TrainingDirectoryClient> _logger = logger;
        public async Task<Trainee?> GetTrainee(TraineeRequest traineeRequest, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve Trainee via HTTP request");

                HttpResponseMessage? response = null;
                try
                {
                    response = await _httpClient.GetAsync($"/trainee/{traineeRequest.SubmissionId}", cancellationToken);
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex.Message, "Network failure while retrieving trainee");
                    throw new HttpRequestException($"Network error retrieving trainee: {ex.Message}", ex);
                }

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                    _logger.LogWarning("API returned error status {StatusCode}: {ErrorBody}", response.StatusCode, errorBody);
                    
                    throw new HttpRequestException($"Error retrieving trainee. Status: {response.StatusCode}. Details: {errorBody}");
                }

                // Success path
                return await response.Content.ReadFromJsonAsync<Trainee>(cancellationToken);
            }
            catch (TaskCanceledException ex) when (ex.CancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning(ex.Message, "Trainee retrieval for {TraineeId} was cancelled.", traineeRequest.SubmissionId);
                throw; 
            }
            catch (Polly.Timeout.TimeoutRejectedException ex)
            {
                _logger.LogError(ex.Message, "Request to retrieve Trainee {TraineeId} timed out via Polly policy.", traineeRequest.SubmissionId);
                throw new TimeoutException("Request Timed out via polly");
            }
            catch (Polly.CircuitBreaker.BrokenCircuitException ex)
            {
                _logger.LogError(ex.Message, "Request to retrieve Trainee {TraineeId} blocked by circuit breaker.", traineeRequest.SubmissionId);
                
                //Fallback returning dummy data
                return new Trainee
                {
                    Id = traineeRequest.SubmissionId,
                    Email = "Unknown ",
                    FirstName = "UnknownFirstName",
                    LastName = "UnknownLastName",
                    Status = Status.Active,
                    TechStack = "Unknown TechStack"
                };

                // return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "An unexpected error occurred while retrieving user {TraineeId}.", traineeRequest.SubmissionId);
                throw new Exception($"An unexpected error occurred while retrieving user {traineeRequest.SubmissionId}.");
            }
        
        }

    }
}