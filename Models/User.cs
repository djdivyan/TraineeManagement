namespace TraineeManagementApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string Username { get; set; } = string.Empty;
        public required string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public required Role Role { get; set; } 

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

    }

    public enum Role
    { Admin, Mentor, Trainee }
}