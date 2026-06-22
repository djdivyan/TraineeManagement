using Models;
using Microsoft.AspNetCore.Mvc;
using TraineeManagementApi.Services;
using TraineeManagementApi.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.VisualBasic;

namespace TraineeManagementApi.Controllers;

[ApiController]
[Route("api/task-assignment")]
[Authorize]
public class TaskAssignmentController(ITaskAssignmentService service) : ControllerBase
{
    private readonly ITaskAssignmentService _service = service;

    [HttpGet]
    public async Task<ActionResult<List<LearningTaskResponse>>> Get()
    {
        List<TaskAssignmentResponse> result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskAssignmentResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        TaskAssignmentResponse taskAssignmentResponse = await _service.GetByIdAsync(id, cancellationToken);
        return Ok(taskAssignmentResponse);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] TaskAssignmentRequest taskAssignmentRequest)
    {

        TaskAssignmentResponse taskAssignmentResponse = await _service.CreateAsync(taskAssignmentRequest);
        return CreatedAtAction(nameof(Get), new { id = taskAssignmentResponse.Id }, taskAssignmentResponse);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskAssignmentRequest updateTaskAssignmentRequest, CancellationToken cancellationToken)
    {
        if (id != updateTaskAssignmentRequest.Id)
            return BadRequest();

        TaskAssignmentResponse result = await _service.UpdateAsync(id,updateTaskAssignmentRequest, cancellationToken);
        return Ok(result);
    }
}
