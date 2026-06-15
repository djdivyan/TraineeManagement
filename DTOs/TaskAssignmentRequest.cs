using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Models;


namespace TraineeManagementApi.DTOs
{
        public class TaskAssignmentRequest
        {

            [Required(ErrorMessage = "Trainee ID is Required")]
            public required int TraineeId { get; set; }

             [Required(ErrorMessage = "Mentor ID is Required")]
            public required int MentorId { get; set; }

             [Required(ErrorMessage = "Learning Task ID is Required")]
            public required int LearningTaskId { get; set; }

            [Required(ErrorMessage = "AssignedDate is required")]
            public DateTime AssignedDate { get; set; }

            [Required(ErrorMessage = "DueDate is required")]
            public DateTime DueDate { get; set; }
            
            [Required(ErrorMessage = "TaskAssignment Status is Required")]
            [EnumDataType(typeof(TaskAssignmentStatus), ErrorMessage = "Status must be valid")]
            [JsonConverter(typeof(JsonStringEnumConverter))]
            public required TaskAssignmentStatus TaskAssignmentStatus { get; set; }
           
            public string? Remarks { get; set; }

    }
}