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
public class AuthController(IUserSevice service) : ControllerBase
{
    private readonly IUserSevice _service = service;

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Post([FromBody] LoginRequest loginRequest)
    {

        // var token = GenerateJwtToken(user);
        // return Ok(new { token });

        if (string.IsNullOrWhiteSpace(loginRequest.Username) || string.IsNullOrWhiteSpace(loginRequest.Password))
        {
            return BadRequest();
        }

        var user = await _service.GetUser(loginRequest);
        if (user is null)
        {
            return BadRequest("User Not Found");
        }

        var isPasswordValid = await _service.ValidatePassword(loginRequest);

        if (!isPasswordValid)
        {
            return Unauthorized("Invalid Password Entered");
        }
        //Generates final response with JWT token
        var response = await _service.Authenticate(loginRequest);
        if (response is null)
        {
            return BadRequest("Something HAppened while generating token");
        }

        return Ok(response);
    }

}
