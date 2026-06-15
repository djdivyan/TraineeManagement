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

    // [HttpGet]
    // public async Task<ActionResult<List<TraineeResponse>>> Get([FromQuery]  string? search)
    // {
    //     var result = await _service.GetAllAsync(search);

    
    //     return Ok(result);
    // }

    [HttpGet("{id}")]
    public async Task<ActionResult<TraineeResponse>> GetById(int id)
    {
        TraineeResponse? trainee = await _service.GetByIdAsync(id);

        if(trainee is null)
            return NotFound();

        return Ok(trainee);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateTraineeRequest createTraineeRequest)
    {

        TraineeResponse? traineeResponse = await _service.CreateAsync(createTraineeRequest);

        return CreatedAtAction(nameof(Get), new { id = traineeResponse.Id }, traineeResponse);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTraineeRequest updateTraineeRequest)
    {
        if (id != updateTraineeRequest.Id)
            return BadRequest();

        TraineeResponse? result = await _service.UpdateAsync(id,updateTraineeRequest);
    
        if(result is null)
            return NotFound();
  
        return Ok(result);
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        bool result = await _service.DeleteAsync(id);
        
        if (result)
            return NoContent();
        else
            return NotFound();
    }


    [HttpGet]
    public async Task<PaginationResponse<TraineeResponse>> Get([FromQuery] PaginationRequest paginationRequest)
    {
        PaginationResponse<TraineeResponse> result = await _service.GetPagedDataAsync(paginationRequest);
        return result;
    }


}
