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
}
