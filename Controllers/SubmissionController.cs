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
public class SubmissionController(ISubmissionService service) : ControllerBase
{
    private readonly ISubmissionService _service = service;

    [HttpGet]
    public async Task<ActionResult<List<SubmissionResponse>>> Get()
    {
        List<SubmissionResponse> result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SubmissionResponse>> GetById(int id)
    {
        SubmissionResponse? submissionResponse = await _service.GetByIdAsync(id);

        if(submissionResponse is null)
            return NotFound();

        return Ok(submissionResponse);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] SubmissionRequest submissionRequest)
    {

        SubmissionResponse? submissionResponse = await _service.CreateAsync(submissionRequest);

        if (submissionResponse is null)
        {
            return BadRequest();
        }

        return CreatedAtAction(nameof(Get), new { id = submissionResponse.Id }, submissionResponse);
    }

}
