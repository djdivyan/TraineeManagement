using System.Text.Json.Serialization;

namespace Models
{
    public class TaskAssignment
    {
        public int Id { get; set; }

        public int TraineeId { get; set; }  
        [JsonIgnore]
        public Trainee Trainee { get; set; } = null!;

        public int MentorId { get; set; }
        [JsonIgnore]
        public Mentor Mentor { get; set; } = null!; 

        public int LearningTaskId { get; set; }
        [JsonIgnore]
        public LearningTask LearningTask { get; set; } = null!;
        public DateTime AssignedDate { get; set; }
        public DateTime DueDate { get; set; }

        public required TaskAssignmentStatus TaskAssignmentStatus { get; set; }
        
        public String? Remarks { get; set; }

        public ICollection<Submission> Submissions {get; set; } = new List<Submission>();
    }

    public enum TaskAssignmentStatus
    {
        Assigned,
        InProgress,
        Submitted,
        Reviewed,
        Completed
    }
}