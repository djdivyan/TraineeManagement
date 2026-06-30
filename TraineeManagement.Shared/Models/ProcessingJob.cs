namespace Models
{
    public class ProcessingJob
    {
        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
        public int SubmissionId { get; set; }
        public int FileId { get; set; }
        public required ProcessingJobStatus ProcessingJobStatus { get; set; }
        public int Attempts { get; set; } = 0;
        public string? ErrorSummary { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }

    public enum ProcessingJobStatus
    {
        Queued,
        Processing,
        Completed,
        Failed
    }
}