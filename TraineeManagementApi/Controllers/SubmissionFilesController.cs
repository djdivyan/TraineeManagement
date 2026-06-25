using Models;
using Microsoft.AspNetCore.Mvc;
using TraineeManagementApi.Services;
using TraineeManagementApi.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.VisualBasic;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Http.Features;
using System.Runtime.Intrinsics.X86;

namespace TraineeManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SubmissionFilesController(ISubmissionFileService service) : ControllerBase
{
    private readonly ISubmissionFileService _service = service;

    [HttpGet]
    [Route("{id}/download")]
    public async Task<IResult> GetById([FromRoute] int id)
    {
        return await _service.GetFileAsync(id);
    }

    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteFileAsync(id);
        return NoContent();
    }
}
