namespace Models
{
    public class LearningTask
    {
        public int Id { get; set; }
        public required string Title { get; set; }

        public required string Description { get; set; }

        public required string ExpectedTechStack { get; set; }
        public DateTime DueDate { get; set; }

        public required LearningTaskStatus LearningTaskStatus { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }


        public ICollection<TaskAssignment> TaskAssignments { get; set; } = new List<TaskAssignment>();
    }

    public enum LearningTaskStatus
    {
        Draft,
        Published,
        Closed
    }
}