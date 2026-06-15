using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Models;

namespace TraineeManagementApi.DTOs
{
    public class SubmissionResponse
    {
        public int Id { get; set; }

        public int TaskAssignmentId { get; set; }

        // public TaskAssignment TaskAssignment { get; set; } = null!;
        public required string SubmissionUrl { get; set; }
        public required string Notes { get; set; }

        public DateTime SubmittedDate { get; set; }

        public required SubmissionStatus SubmissionStatus { get; set; }
        
    }
}