using Microsoft.AspNetCore.Mvc;
using TraineeManagementApi.Services;
using TraineeManagementApi.DTOs;
using Microsoft.AspNetCore.Authorization;
using TraineeManagement.Shared.Models;

namespace TraineeManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReviewController(IReviewService service, IAuthorizationService authService) : ControllerBase
{
    private readonly IReviewService _service = service;
    private readonly IAuthorizationService _authService = authService;

    [HttpGet]
    [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Mentor)}")]
    public async Task<ActionResult<List<ReviewResponse>>> Get()
    {
        List<ReviewResponse> result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReviewResponse>> GetById(int id)
    {
        var rawReview = await _service.GetRawByIdAsync(id);
        if (rawReview is null) 
        {
            return NotFound($"Review with ID {id} not found.");
        }

        // Check authorization against the centralized engine policy
        var authResult = await _authService.AuthorizeAsync(User, rawReview, "MustOwnResource");
        if (!authResult.Succeeded)
        {
            return Forbid(); 
        }

        ReviewResponse reviewResponse = await _service.GetByIdAsync(id);
        return Ok(reviewResponse);
    }

    [HttpPost]
    [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Mentor)}")]
    public async Task<IActionResult> Post([FromBody] ReviewRequest reviewRequest)
    {
        ReviewResponse reviewResponse = await _service.CreateAsync(reviewRequest);
        return CreatedAtAction(nameof(Get), new { id = reviewResponse.Id }, reviewResponse);
    }
}
