using TraineeManagement.Shared.Contracts;

namespace Models
{
    public class Trainee: IOwnedResource
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public required string Email { get; set; }
        public required string TechStack { get; set; }
        public required Status Status { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }


        public ICollection<TaskAssignment> TaskAssignments { get; set; } = new List<TaskAssignment>();
        public int GetOwnerTraineeId()
        {
            return Id;
        }
    }

    public enum Status
    {
        Active,
        Inactive,
        Completed
    }
}