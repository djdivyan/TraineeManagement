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
public class SubmissionController(ISubmissionService service) : ControllerBase
{
    private readonly ISubmissionService _service = service;

    [HttpGet]
    public async Task<ActionResult<List<SubmissionResponse>>> Get()
    {
        List<SubmissionResponse> result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SubmissionResponse>> GetById(int id)
    {
        SubmissionResponse submissionResponse = await _service.GetByIdAsync(id);
        return Ok(submissionResponse);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] SubmissionRequest submissionRequest)
    {
        SubmissionResponse submissionResponse = await _service.CreateAsync(submissionRequest);
        return CreatedAtAction(nameof(Get), new { id = submissionResponse.Id }, submissionResponse);
    }

    [HttpPost]
    [Route("{submissionid}/files")]
    public async Task<IActionResult> UploadFile([FromRoute]int submissionid, [FromForm]SubmissionFileRequestDTO request)
    {
        if (submissionid != request.SubmissionId)
        {
            return BadRequest("Submission ID mismatch");
        }
        if (request.File == null || request.File.Length == 0 )
            return BadRequest("No file uploaded.");
        try
        {  
            SubmissionFileResponseDTO? savedFilePath = await _service.SaveFileAsync(submissionid,request);
            return Ok(new { Message = "File uploaded successfully.", FilePath = savedFilePath });
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }

        [HttpGet]
        [Route("{submissionid}/summary")]
        public async Task<SubmissionSummaryDTO> GetSubmissionSummary([FromRoute]int submissionid, CancellationToken cancellationToken)
        {
            return await _service.GetSubmissionSummaryAsync(submissionid,cancellationToken);
        }
}
