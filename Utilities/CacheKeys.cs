namespace TraineeManagementApi.Utilities
{
    public static class CacheKeys
    {
        public static string Trainee(int id)
        {
            return $"trainees:{id}";
        }

        public const string AllTrainees = "trainees";
        public const string AllTaskAssignment = "task_assignment";

        public static string SubmissionSummary(int id)
        {
            return $"submission-summary:{id}";
        }

        public static string TaskAssignment(int id)
        {
            return $"task_assignment:{id}";
        }

    }
}