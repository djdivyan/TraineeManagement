using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TraineeManagementApi.DTOs;
using TraineeManagementApi.Models;

namespace TraineeManagementApi.Services
{
    class LoginService(AppDbContext dbContext, IConfiguration configuration, ILogger<LoginService> logger) : ILoginService
    {
        private readonly ILogger<LoginService> _logger = logger;
        private readonly AppDbContext _dbContext = dbContext; 

        private readonly IConfiguration _configuration = configuration;

        public async Task<AuthResponse<LoginResponse?>> Authenticate(LoginRequest loginRequest)
        {
            AuthResponse<LoginResponse?> authResponse = new();
            
            //Check if User Present
            User? user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == loginRequest.Username);
            if (user is null)
            {

                authResponse.Exception = "User Not Found";
                authResponse.StatusCode = 10001;
                _logger.LogError("Login Failed : Unable to Find user {username}",loginRequest.Username);
                return authResponse;
            }

            //Validate Password
            PasswordHasher<User> hasher = new PasswordHasher<User>();
            PasswordVerificationResult result = hasher.VerifyHashedPassword(user,user.PasswordHash,loginRequest.Password);
            if (result == PasswordVerificationResult.Failed)
            {

                authResponse.Exception = "Password is Invalid";
                authResponse.StatusCode = 1002;
                _logger.LogError("Login Failed : Incorrect password for user {username}",loginRequest.Username);
                return authResponse;
            }
            
            //If Everything Works then Generae JWT Token 
            string issuer = _configuration["JwtConfig:Issuer"]!;
            string audience = _configuration["JwtConfig:Audience"]!;
            string key = _configuration["JwtConfig:Key"]!;
        
            int tokenValidityMins = _configuration.GetValue<int>("JwtConfig:TokenValidityMins");
            DateTime tokenExpiryTimeStamp = DateTime.UtcNow.AddMinutes(tokenValidityMins);
            
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name,user.Username),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                NotBefore = DateTime.UtcNow,
                Expires = tokenExpiryTimeStamp,
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                SecurityAlgorithms.HmacSha256),
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
            string accessToken = tokenHandler.WriteToken(securityToken);
            
            //Build Final response
            authResponse.StatusCode = 0;
            authResponse.LoginResponse =  new LoginResponse
            {
                Token = accessToken,
                ExpiresIn = (int)tokenExpiryTimeStamp.Subtract(DateTime.UtcNow).TotalSeconds,
                ResponseUser = new()
                {
                    Id = user.Id,
                    Username = user.Username,
                    Role = user.Role
                }
            };
            
            _logger.LogInformation("Login successful for user {username}",user.Username);
            return authResponse;
        }
    }
}