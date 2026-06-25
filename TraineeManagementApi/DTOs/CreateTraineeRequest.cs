using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Models;

namespace TraineeManagementApi.DTOs
{
        public class CreateTraineeRequest
        {

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
            public required string TechStack { get; set; }


            [Required(ErrorMessage = "Status is Required")]
            [EnumDataType(typeof(Status), ErrorMessage = "Status must be valid")]
            [JsonConverter(typeof(JsonStringEnumConverter))]
            public required Status Status { get; set; }

    }
}