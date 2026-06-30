using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using TraineeManagementApi.Services;

namespace TraineeManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProcessingJobController(IProcessingJobService service) : ControllerBase
{
    private readonly IProcessingJobService _service = service;

    [HttpGet]
    [Route("{id}")]
    public async Task<ProcessingJob> GetById([FromRoute] int id)
    {
        return await _service.GetJobByIdAsync(id);
    }
    
    [HttpPost]
    [Route("{id}/retry")]
    public async Task<ActionResult<ProcessingJob>> Retry([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        ProcessingJob job =  await _service.RetryJob(id,cancellationToken);
        return Ok(job);
    }

}
