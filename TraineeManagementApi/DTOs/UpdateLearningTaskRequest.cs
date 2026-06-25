using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Models;

namespace TraineeManagementApi.DTOs
{
        public class UpdateLearningTaskRequest
        {
            [Required(ErrorMessage = "Id is Required")]
            public int Id { get; set; }
            [Required(ErrorMessage = "Title is Required")]
            public required string Title { get; set; }

            [Required(ErrorMessage = "Description is Required")]
            public required string Description { get; set; }

            [Required(ErrorMessage = "ExpectedTechStack is Required")]
            public required string ExpectedTechStack { get; set; }

            [Required(ErrorMessage = "DueDate is required")]
            public DateTime DueDate { get; set; }

            [Required(ErrorMessage = "Task Status is Required")]
            [EnumDataType(typeof(LearningTaskStatus), ErrorMessage = "Status must be valid")]
            [JsonConverter(typeof(JsonStringEnumConverter))]
            public required LearningTaskStatus LearningTaskStatus { get; set; }

    }
}