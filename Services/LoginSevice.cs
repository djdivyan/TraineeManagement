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
    class LoginService : ILoginService
    { 
        private readonly AppDbContext _dbContext; 

        private readonly IConfiguration _configuration;

        public LoginService(AppDbContext dbContext,IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }
        

        public async Task<User?> GetUser(LoginRequest loginRequest)
        {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == loginRequest.Username);
                return user;
        }

        public async Task<bool> ValidatePassword(LoginRequest loginRequest)
        {
           
           
         var user = await GetUser(loginRequest);
         if (user is null)
         {
            return false;
         }
         var hasher = new PasswordHasher<User>();
         var result = hasher.VerifyHashedPassword(user,user.PasswordHash,loginRequest.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            return false;
        }

        return true;

        }


        public async Task<LoginResponse?> Authenticate(LoginRequest loginRequest)
        {
            var user = await GetUser(loginRequest);
            if (user is null)
            {
                Console.WriteLine("User Not Found");
                return null;
            }
            var issuer = _configuration["JwtConfig:Issuer"]!;
            var audience = _configuration["JwtConfig:Audience"]!;
            var key = _configuration["JwtConfig:Key"]!;
        
            var tokenValidityMins = _configuration.GetValue<int>("JwtConfig:TokenValidityMins");
            var tokenExpiryTimeStamp = DateTime.UtcNow.AddMinutes(tokenValidityMins);
            Console.WriteLine(tokenExpiryTimeStamp+"Validity Mins"+tokenValidityMins+"Current"+DateTime.UtcNow);
            
            var tokenDescriptor = new SecurityTokenDescriptor
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

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(securityToken);
            return new LoginResponse
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
        }



        //     public async Task<LoginResponse?> login(LoginRequest loginRequest)
        //     {
        //         var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == loginRequest.Username);
        //         if (user is null)
        //         {
        //             return null;
        //         }

        //         var hasher = new PasswordHasher<User>();
        //         var hashPass = user.PasswordHash;
        //         user.PasswordHash = "";
        //         var result = hasher.VerifyHashedPassword(user,hashPass,loginRequest.Password);
        //     if (result == PasswordVerificationResult.Failed)
        //     {
        //         return Unauthorized();
        //     }



        //    return new LoginResponse
        //         {
        //         Id = User.Id,
        //         Username = ,
        //         Role = 
        //         };


        //     }
    }
}