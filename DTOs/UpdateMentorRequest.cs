using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Models;

namespace TraineeManagementApi.DTOs
{
        public class UpdateMentorRequest
        {
            [Required(ErrorMessage = "Id is Required")]
            public int Id { get; set; }

            [Required(ErrorMessage = "First Name is Required")]
            [MaxLength(50, ErrorMessage = "{0} can have a max of {1} characters")]
            public required string FirstName { get; set; }

            [Required(ErrorMessage = "Last Name is Required")]
            [MaxLength(50, ErrorMessage = "{0} can have a max of {1} characters")]
            public required string LastName { get; set; }

            [Required(ErrorMessage = "Email is Required")]
            [EmailAddress(ErrorMessage = "Invalid Email Address")]
            public required string Email { get; set; }


            [Required(ErrorMessage = "TechStack is Required")]
            public required string Expertise { get; set; }


            [Required(ErrorMessage = "Mentor Status is Required")]
            [EnumDataType(typeof(MentorStatus), ErrorMessage = "Status must be valid")]
            [JsonConverter(typeof(JsonStringEnumConverter))]
            public required MentorStatus MentorStatus { get; set; }

    }
}