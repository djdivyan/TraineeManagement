namespace SubmissionProcessingWorker.DTOs
{
    public class TraineeRequest
    {
        public int SubmissionId { get; set; }
        public string CorrelationId { get; set; } = null!;
    }
}