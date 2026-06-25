using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Models;

namespace TraineeManagementApi.DTOs
{
    public class TraineeResponse
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public required string Email { get; set; }
        public required string TechStack { get; set; }
        
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public required Status Status { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
}