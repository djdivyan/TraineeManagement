using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Models;


namespace TraineeManagementApi.DTOs
{
        public class SubmissionRequest
        {

            [Required(ErrorMessage = "TaskAssignment ID is Required")]
            public required int TaskAssignmentId { get; set; }

            [Required(ErrorMessage = "Submission URL is Required")]
            public required string SubmissionUrl { get; set; }

            [Required(ErrorMessage = "Notes is Required")]
            public required string Notes { get; set; }
            
            [Required(ErrorMessage = "Submission Status is Required")]
            [EnumDataType(typeof(SubmissionStatus), ErrorMessage = "Status must be valid")]
            [JsonConverter(typeof(JsonStringEnumConverter))]
            public required SubmissionStatus SubmissionStatus { get; set; }
    }
}