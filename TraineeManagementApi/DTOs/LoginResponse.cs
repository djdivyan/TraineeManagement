using System.Text.Json.Serialization;
using Models;
using TraineeManagement.Shared.Models;

namespace TraineeManagementApi.DTOs
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }  
        public required ResponseUser ResponseUser { get; set; }
        
    }

    public class ResponseUser
    {
        public int Id { get; set; }
        public required string Username { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public required Role Role { get; set; }
    }
}