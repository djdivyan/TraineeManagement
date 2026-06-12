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

        // var token = GenerateJwtToken(user);
        // return Ok(new { token });

        if (string.IsNullOrWhiteSpace(loginRequest.Username) || string.IsNullOrWhiteSpace(loginRequest.Password))
        {
            _logger.LogError("Login Failed : username or password is NULL");
            return BadRequest();
        }

        var user = await _service.GetUser(loginRequest);
        if (user is null)
        {
            _logger.LogError("Login Failed : Unable to Find user {username}",loginRequest.Username);
            return BadRequest("User Not Found");
        }

        var isPasswordValid = await _service.ValidatePassword(loginRequest);

        if (!isPasswordValid)
        {
            _logger.LogError("Login Failed : Incorrect password for user {username}",loginRequest.Username);
            return Unauthorized("Invalid Password Entered");
        }
        //Generates final response with JWT token
        var response = await _service.Authenticate(loginRequest);

        _logger.LogInformation("Login successful for user {username}",user.Username);
        return Ok(response);
    }

}
