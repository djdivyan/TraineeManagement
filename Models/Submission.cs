using System.Text.Json.Serialization;

namespace Models
{
    public class Submission
    {
        public int Id { get; set; }

        public int TaskAssignmentId { get; set; }
          
        [JsonIgnore]
        public TaskAssignment TaskAssignment { get; set; } = null!;
        public required string SubmissionUrl { get; set; }
        public required string Notes { get; set; }

        public DateTime SubmittedDate { get; set; }

        public required SubmissionStatus SubmissionStatus { get; set; }
        
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<SubmissionFile> SubmissionFiles {get; set;} = new List<SubmissionFile>();
    }

    public enum SubmissionStatus
    {
        Submitted,
        Resubmitted
    }
}