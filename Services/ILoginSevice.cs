using Models;
using TraineeManagementApi.DTOs;
using TraineeManagementApi.Models;

namespace TraineeManagementApi.Services
{
    public interface ILoginService
    {
        Task<AuthResponse<LoginResponse?>> Authenticate(LoginRequest loginRequest);
    } 
}