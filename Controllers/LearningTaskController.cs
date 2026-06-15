using Models;
using Microsoft.AspNetCore.Mvc;
using TraineeManagementApi.Services;
using TraineeManagementApi.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.VisualBasic;

namespace TraineeManagementApi.Controllers;

[ApiController]
[Route("api/learning-tasks")]
[Authorize]
public class LearningTaskController(ILearningTaskService service) : ControllerBase
{
    private readonly ILearningTaskService _service = service;

    [HttpGet]
    public async Task<ActionResult<List<LearningTaskResponse>>> Get([FromQuery]  string? search)
    {
        var result = await _service.GetAllAsync(search);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LearningTaskResponse>> GetById(int id)
    {
        LearningTaskResponse? learningTaskResponse = await _service.GetByIdAsync(id);

        if(learningTaskResponse is null)
            return NotFound();

        return Ok(learningTaskResponse);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] LearningTaskRequest learningTaskRequest)
    {

        LearningTaskResponse? learningTaskResponse = await _service.CreateAsync(learningTaskRequest);

        return CreatedAtAction(nameof(Get), new { id = learningTaskResponse.Id }, learningTaskResponse);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateLearningTaskRequest updateLearningTaskRequest)
    {
        if (id != updateLearningTaskRequest.Id)
            return BadRequest();

        LearningTaskResponse? result = await _service.UpdateAsync(id,updateLearningTaskRequest);
    
        if(result is null)
            return NotFound();
  
        return Ok(result);
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        
        if (result)
            return NoContent();
        else
            return NotFound();
    }


    // [HttpGet]
    // public async Task<PaginationResponse<TraineeResponse>> Get([FromQuery] PaginationRequest paginationRequest)
    // {
    //     PaginationResponse<TraineeResponse> result = await _service.GetPagedDataAsync(paginationRequest);
    //     return result;
    // }


}
