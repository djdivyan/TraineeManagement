using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Models;


namespace TraineeManagementApi.DTOs
{
        public class ReviewRequest
        {

            [Required(ErrorMessage = "Submission ID is Required")]
            public required int SubmissionId { get; set; }

            [Required(ErrorMessage = "Mentor ID is Required")]
            public required int MentorId { get; set; }

            [Required(ErrorMessage = "FeedBack is Required")]
            public required string FeedBack { get; set; }

            public int? Score { get; set; }
            
            [Required(ErrorMessage = "Review Status is Required")]
            [EnumDataType(typeof(ReviewStatus), ErrorMessage = "Status must be valid")]
            [JsonConverter(typeof(JsonStringEnumConverter))]
            public required ReviewStatus ReviewStatus { get; set; }
           
            [Required(ErrorMessage = "Reviewed Date is Required")]
            public DateTime ReviewedDate { get; set; }
    }
}