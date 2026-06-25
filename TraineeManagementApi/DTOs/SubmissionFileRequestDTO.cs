using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Models;


namespace TraineeManagementApi.DTOs
{
        public class SubmissionFileRequestDTO
        {

            [Required(ErrorMessage = "UploadedByUser is Required")]
            public required int UploadedByUser { get; set; }
            
            [Required(ErrorMessage = "SubmissionId is Required")]
            public required int SubmissionId { get; set; }

            [Required(ErrorMessage = "File is Required")]
            public required IFormFile File { get; set; }
    }
}