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
    [Route("trainee/{SubmissionId}")]
    public async Task<ActionResult<Trainee>> GetById([FromRoute]int SubmissionId,[FromQuery] string correlationId, CancellationToken cancellationToken)
    {
        Console.WriteLine($"correlationId: {correlationId} Recieved a Id {SubmissionId}");
        Trainee? trainee = await _service.GetTraineeAsync(SubmissionId,correlationId, cancellationToken);
        if (trainee == null)
        {
            return NotFound("Trainee Could not be found via submissionID");
        }
        return Ok(trainee);
    }
}
