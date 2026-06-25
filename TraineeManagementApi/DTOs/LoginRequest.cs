using System.ComponentModel.DataAnnotations;


namespace TraineeManagementApi.DTOs
{
        public class LoginRequest
        {

            [Required(ErrorMessage = "First Name is Required")]
            public string Username { get; set; } = string.Empty;

            [Required(ErrorMessage = "Password is Required")]
            [MinLength(5, ErrorMessage = "{0} must have a min of {1} characters")]
            public string Password { get; set; } = string.Empty;


    }
}