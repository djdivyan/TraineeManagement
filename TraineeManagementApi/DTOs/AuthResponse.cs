using System.Text.Json.Serialization;
using Models;
using TraineeManagement.Shared.Models;

namespace TraineeManagementApi.DTOs
{
    public class AuthResponse<T>
    {
        public T? LoginResponse { get; set; } 
        public object? Exception {get; set; }
        public int StatusCode { get; set; }               
    }

    public static class AuthStatusCodes
    {
        public const int Success = 0;
        public const int UserNotFound = 10001;
        public const int InvalidPassword = 10002;

    }
}