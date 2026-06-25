using System.Text.Json.Serialization;

namespace Models
{
    public class Review
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

    public enum ReviewStatus
    {
        Accepted,
        ChangesRequired,
        Rejected
    }
}