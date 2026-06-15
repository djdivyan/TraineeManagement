using Models;
using Microsoft.AspNetCore.Mvc;
using TraineeManagementApi.Services;
using TraineeManagementApi.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using TraineeManagementApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace TraineeManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(ILoginService service,ILogger<AuthController> logger) : ControllerBase
{
    private readonly ILoginService _service = service;
    private readonly ILogger<AuthController> _logger = logger;

   

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Post([FromBody] LoginRequest loginRequest)
    {
        if (string.IsNullOrWhiteSpace(loginRequest.Username) || string.IsNullOrWhiteSpace(loginRequest.Password))
        {
            _logger.LogError("Login Failed : username or password is NULL");
            return BadRequest();
        }
        
        //Generates final response with JWT token
        AuthResponse<LoginResponse?> response = await _service.Authenticate(loginRequest);

        return Ok(response);
    }

}
