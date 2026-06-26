using Microsoft.AspNetCore.Mvc;
using Models;
using TrainingDirectory.Api.Services;

namespace TrainingDirectory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TrainingDirectoryController(ITrainingDirectoryService service) : ControllerBase
{
    private readonly ITrainingDirectoryService _service = service;

    [HttpGet]
    [Route("/trainee/{SubmissionId}")]
    public async Task<ActionResult<Trainee>> GetById(int SubmissionId, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Recieved a Id {SubmissionId}");
        Trainee? trainee = await _service.GetTraineeAsync(SubmissionId, cancellationToken);
        if (trainee == null)
        {
            return NotFound("Trainee Could not be found via submissionID");
        }
        return Ok(trainee);
    }
}
