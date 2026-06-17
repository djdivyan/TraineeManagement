using Models;
using Microsoft.AspNetCore.Mvc;
using TraineeManagementApi.Services;
using TraineeManagementApi.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.VisualBasic;

namespace TraineeManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReviewController(IReviewService service) : ControllerBase
{
    private readonly IReviewService _service = service;

    [HttpGet]
    public async Task<ActionResult<List<ReviewResponse>>> Get()
    {
        List<ReviewResponse> result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReviewResponse>> GetById(int id)
    {
        ReviewResponse reviewResponse = await _service.GetByIdAsync(id);
        return Ok(reviewResponse);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ReviewRequest reviewRequest)
    {
        ReviewResponse reviewResponse = await _service.CreateAsync(reviewRequest);
        return CreatedAtAction(nameof(Get), new { id = reviewResponse.Id }, reviewResponse);
    }
}
