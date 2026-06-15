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
    public async Task<ActionResult<TaskAssignmentResponse>> GetById(int id)
    {
        TaskAssignmentResponse? taskAssignmentResponse = await _service.GetByIdAsync(id);

        if(taskAssignmentResponse is null)
            return NotFound();

        return Ok(taskAssignmentResponse);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] TaskAssignmentRequest taskAssignmentRequest)
    {

        TaskAssignmentResponse? taskAssignmentResponse = await _service.CreateAsync(taskAssignmentRequest);

        if (taskAssignmentResponse is null)
        {
            return BadRequest();
        }

        return CreatedAtAction(nameof(Get), new { id = taskAssignmentResponse.Id }, taskAssignmentResponse);
    }


    [HttpPut("{id}/status")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskAssignmentRequest updateTaskAssignmentRequest)
    {
        if (id != updateTaskAssignmentRequest.Id)
            return BadRequest();

        TaskAssignmentResponse? result = await _service.UpdateAsync(id,updateTaskAssignmentRequest);
    
        if(result is null)
            return NotFound();
  
        return Ok(result);
    }


    // [HttpDelete("{id}")]
    // public async Task<IActionResult> Delete(int id)
    // {
    //     bool result = await _service.DeleteAsync(id);
        
    //     if (result)
    //         return NoContent();
    //     else
    //         return NotFound();
    // }


    // [HttpGet]
    // public async Task<PaginationResponse<TraineeResponse>> Get([FromQuery] PaginationRequest paginationRequest)
    // {
    //     PaginationResponse<TraineeResponse> result = await _service.GetPagedDataAsync(paginationRequest);
    //     return result;
    // }


}
