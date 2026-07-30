using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Models;

namespace TraineeManagementApi.DTOs
{
    public class SubmissionSummaryDTO
    {

        public int TaskAssignmentId { get; set; }
        public required string SubmissionUrl { get; set; }
        public required string Notes { get; set; }
        public DateTime SubmittedDate { get; set; }
        public required SubmissionStatus SubmissionStatus { get; set; }

        public int? UserId { get; set; }
    }
}