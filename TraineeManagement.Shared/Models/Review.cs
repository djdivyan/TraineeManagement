using System.Text.Json.Serialization;
using TraineeManagement.Shared.Contracts;

namespace Models
{
    public class Review: IOwnedResource
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

        public int GetOwnerTraineeId()
        {
            return Submission?.TaskAssignment?.Trainee?.Id ?? 0;
        }


    }

    public enum ReviewStatus
    {
        Accepted,
        ChangesRequired,
        Rejected
    }
}