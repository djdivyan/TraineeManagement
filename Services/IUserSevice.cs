using Models;
using TraineeManagementApi.DTOs;
using TraineeManagementApi.Models;

namespace TraineeManagementApi.Services
{
    public interface IUserSevice
    {
        Task<User?> GetUser(LoginRequest loginRequest);
        Task<bool> ValidatePassword(LoginRequest loginRequest);

        Task<LoginResponse?> Authenticate(LoginRequest loginRequest);
    } 
}