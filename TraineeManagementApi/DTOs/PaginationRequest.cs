using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Models;

namespace TraineeManagementApi.DTOs
{
    public class PaginationRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        
        [EnumDataType(typeof(Status), ErrorMessage ="Status must be Valid")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Status? Status { get; set; }
    }
}