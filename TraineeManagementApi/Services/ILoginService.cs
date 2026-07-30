using Models;
using TraineeManagementApi.DTOs;
using TraineeManagement.Shared.Models;

namespace TraineeManagementApi.Services
{
    public interface ILoginService
    {
        Task<AuthResponse<LoginResponse?>> Authenticate(LoginRequest loginRequest);
    } 
}