using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Models;


namespace TraineeManagementApi.DTOs
{
    public class SubmissionFileResponseDTO
    {
        public int Id { get; set; }
        public Guid TrackingIdentifier { get; set; }
        public int SubmissionId { get; set; }
        public required string  OriginalFileName { get; set; }
        public required string  GeneratedStorageName { get; set; }
        public required string  ContentType { get; set; }
        public required long Size { get; set; }
        public required string  Checksum { get; set; }
        public required int UploadedByUser { get; set; }
        public DateTime Timestamp { get; set; }
    }
}