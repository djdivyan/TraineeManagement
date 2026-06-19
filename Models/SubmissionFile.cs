using System.Text.Json.Serialization;

namespace Models
{
    public class SubmissionFile
    {
        public int Id { get; set; }

        public int SubmissionId { get; set; }
          
        [JsonIgnore]
        public Submission Submission { get; set; } = null!;
        public required string  OriginalFileName { get; set; }
        public required string  GeneratedStorageName { get; set; }
        public required string  ContentType { get; set; }
        public required long Size { get; set; }
        public required string  Checksum { get; set; }
        public required int UploadedByUser { get; set; }

        public DateTime Timestamp { get; set; }

    }
}