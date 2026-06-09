using Models;
using Microsoft.AspNetCore.Mvc;

namespace TraineeManagementApi.Controllers;

[ApiController]
[Route("[controller]")]
public class TraineesController : ControllerBase
{
    private static readonly List<Trainee> Trainees = [];
    private static int index = 0;




    [HttpGet]
    public ActionResult<List<Trainee>> Get()
    {
        return Ok(
            Trainees
        );
    }

    [HttpGet("{id}")]
    public ActionResult<Trainee> Get(int id)
    {
        var trainee = Trainees.FirstOrDefault(t => t.Id == id);

        if(trainee == null)
            return NotFound();

        return Ok(trainee);
    }

    [HttpPost]
    public IActionResult Post([FromBody] Trainee trainee)
    {
        // var newTrainee = new
        // {
        //     Id = index++,
        //     FirstName = "Div",
        //     LastName = "Jain",
        //     Email = "dj@gmail.com", 
        //     TechStack = "Java", 
        //     Status = "Active",
        //     CreatedDate = DateTime.Now, 
        //     UpdatedDate = DateTime.Now

        // };
        trainee.Id = index++;
        trainee.CreatedDate = DateTime.Now;
        trainee.UpdatedDate = DateTime.Now;

        Trainees.Add(trainee);

        return CreatedAtAction(nameof(Get), new { id = trainee.Id }, trainee);
    }
}
