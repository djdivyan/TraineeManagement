using Models;
using TraineeManagementApi.DTOs;
using TraineeManagementApi.Models;

namespace TraineeManagementApi.Services
{
    public interface ILoginService
    {
        Task<User?> GetUser(LoginRequest loginRequest);
        Task<bool> ValidatePassword(LoginRequest loginRequest);

        Task<LoginResponse?> Authenticate(LoginRequest loginRequest);
    } 
}