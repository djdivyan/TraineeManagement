using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Models;

namespace TraineeManagementApi.DTOs
{
        public class UpdateTaskAssignmentRequest
        {
            [Required(ErrorMessage = "Id is Required")]
            public int Id { get; set; }
            
            [Required(ErrorMessage = "TaskAssignment Status is Required")]
            [EnumDataType(typeof(TaskAssignmentStatus), ErrorMessage = "Status must be valid")]
            [JsonConverter(typeof(JsonStringEnumConverter))]
            public required TaskAssignmentStatus TaskAssignmentStatus { get; set; }
           
    }
}