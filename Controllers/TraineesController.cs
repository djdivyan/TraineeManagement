using Models;
using Microsoft.AspNetCore.Mvc;
using TraineeManagementApi.Services;
using TraineeManagementApi.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;

namespace TraineeManagementApi.Controllers;

[ApiController]
[Route("[controller]")]
public class TraineesController : ControllerBase
{
    private readonly ITraineeService _service;

    public TraineesController(ITraineeService service)
    {
        _service = service;
    }


    [HttpGet]
    public ActionResult<List<Trainee>> Get()
    {
        return Ok(_service.GetAll());
    }

    [HttpGet("{id}")]
    public ActionResult<Trainee> Get(int id)
    {
        var trainee = _service.GetById(id);

        if(trainee is null)
            return NotFound();

        return Ok(trainee);
    }

    [HttpPost]
    public IActionResult Post([FromBody] CreateTraineeRequest createTraineeRequest)
    {

        var traineeResponse = _service.Create(createTraineeRequest);

        return CreatedAtAction(nameof(Get), new { id = traineeResponse.Id }, traineeResponse);
    }


    [HttpPut("{id}")]
    public ActionResult<TraineeResponse> Update(int id, [FromBody] UpdateTraineeRequest updateTraineeRequest)
    {
    if (id != updateTraineeRequest.Id)
        return BadRequest();
           
    var existingTrainee = _service.GetById(id);
    if(existingTrainee is null)
        return NotFound();

    return _service.Update(id,updateTraineeRequest);
    }


    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
    if (_service.Delete(id))
    {
        return NoContent();
    }else
    {
        return NotFound();
    }   
    }


}
