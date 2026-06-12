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
public class MentorsController(IMentorService service) : ControllerBase
{
    private readonly IMentorService _service = service;

    [HttpGet]
    public async Task<ActionResult<List<TraineeResponse>>> Get([FromQuery]  string? search)
    {
        var result = await _service.GetAllAsync(search);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TraineeResponse>> GetById(int id)
    {
        MentorResponse? mentor = await _service.GetByIdAsync(id);

        if(mentor is null)
            return NotFound();

        return Ok(mentor);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] MentorRequest mentorRequest)
    {

        MentorResponse? mentorResponse = await _service.CreateAsync(mentorRequest);

        return CreatedAtAction(nameof(Get), new { id = mentorResponse.Id }, mentorResponse);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMentoreRequest updateMentoreRequest)
    {
        if (id != updateMentoreRequest.Id)
            return BadRequest();

        MentorResponse? result = await _service.UpdateAsync(id,updateMentoreRequest);
    
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
