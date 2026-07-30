using System.Text.Json.Serialization;
using TraineeManagement.Shared.Contracts;

namespace Models
{
    public class Submission: IOwnedResource 
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
        public int GetOwnerTraineeId()
        {
            return TaskAssignment?.Trainee?.Id ?? 0;
        }
    }

    public enum SubmissionStatus
    {
        Submitted,
        Resubmitted
    }
}