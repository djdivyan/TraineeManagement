using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Models;


namespace TraineeManagementApi.DTOs
{
    public class ReviewResponse
    {
        public int Id { get; set; }

        public int SubmissionId { get; set; }

        [JsonIgnore]
        public Submission Submission { get; set; } = null!;

        public int MentorId { get; set; }
        
        [JsonIgnore]
        public Mentor Mentor { get; set; } = null!;

        public required string Feedback { get; set; }
        public int? Score { get; set; }

        public required ReviewStatus ReviewStatus { get; set; }

        public DateTime ReviewedDate { get; set; }

    }
}