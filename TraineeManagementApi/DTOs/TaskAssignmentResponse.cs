using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Models;

namespace TraineeManagementApi.DTOs
{
    public class TaskAssignmentResponse
    {
        public int Id { get; set; }
        public int TraineeId { get; set; }
        public Trainee Trainee { get; set; } = null!;

        public int MentorId { get; set; }
        public Mentor Mentor { get; set; } = null!; 

        public int LearningTaskId { get; set; }
        public LearningTask LearningTask { get; set; } = null!;

        public DateTime AssignedDate { get; set; }
        public DateTime DueDate { get; set; }
        public required TaskAssignmentStatus TaskAssignmentStatus { get; set; }
        public String? Remarks { get; set; }

    }
}