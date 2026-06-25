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
public class TraineesController(ITraineeService service) : ControllerBase
{
    private readonly ITraineeService _service = service;


    [HttpGet("{id}")]
    public async Task<ActionResult<TraineeResponse>> GetById(int id,CancellationToken cancellationToken)
    {
        
        TraineeResponse? trainee = await _service.GetByIdAsync(id,cancellationToken);
        return Ok(trainee);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateTraineeRequest createTraineeRequest)
    {
        TraineeResponse traineeResponse = await _service.CreateAsync(createTraineeRequest);
        return CreatedAtAction(nameof(Get), new { id = traineeResponse.Id }, traineeResponse);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTraineeRequest updateTraineeRequest,CancellationToken cancellationToken )
    {
        if (id != updateTraineeRequest.Id)
            return BadRequest();

        TraineeResponse result = await _service.UpdateAsync(id,updateTraineeRequest,cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(id,cancellationToken);
        return NoContent();
    }

    [HttpGet]
    public async Task<PaginationResponse<TraineeResponse>> Get([FromQuery] PaginationRequest paginationRequest)
    {
        PaginationResponse<TraineeResponse> result = await _service.GetPagedDataAsync(paginationRequest);
        return result;
    }
}
