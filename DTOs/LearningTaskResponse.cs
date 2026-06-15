using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Models;

namespace TraineeManagementApi.DTOs
{
    public class LearningTaskResponse
    {
        public int Id { get; set; }
        public required string Title { get; set; }

        public required string Description { get; set; }

        public required string ExpectedTechStack { get; set; }

        public required DateTime DueDate { get; set; }


        [JsonConverter(typeof(JsonStringEnumConverter))]
        public required LearningTaskStatus LearningTaskStatus { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
}