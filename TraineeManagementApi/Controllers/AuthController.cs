using Microsoft.AspNetCore.Mvc;
using TraineeManagementApi.DTOs;
using Microsoft.AspNetCore.Authorization;
using TraineeManagementApi.Services;

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

        // Response.Headers.Append("Authorization Bearer", response.LoginResponse.Token);
        return Ok(response);
    }

}
