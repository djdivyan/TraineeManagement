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
using TraineeManagement.Shared.Models;

namespace TraineeManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SubmissionController(ISubmissionService service, IAuthorizationService authService) : ControllerBase
{
    private readonly ISubmissionService _service = service;
    private readonly IAuthorizationService _authService = authService;


    [HttpGet]
    [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Mentor)}")]
    public async Task<ActionResult<List<SubmissionResponse>>> Get()
    {
        List<SubmissionResponse> result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SubmissionResponse>> GetById(int id)
    {
        var rawSubmission = await _service.GetRawByIdAsync(id);
        if (rawSubmission is null) return NotFound();

        var authResult = await _authService.AuthorizeAsync(User, rawSubmission, "MustOwnResource");
        if (!authResult.Succeeded) return Forbid();

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
    public async Task<IActionResult> UploadFile([FromRoute]int submissionid, [FromForm]SubmissionFileRequestDTO request, CancellationToken cancellationToken)
    {
        if (submissionid != request.SubmissionId)
        {
            return BadRequest("Submission ID mismatch");
        }  
        if (request.File == null || request.File.Length == 0 )
            return BadRequest("No file uploaded.");

        var rawSubmission = await _service.GetRawByIdAsync(submissionid);
        if (rawSubmission is null) return NotFound();

        // Centralized guard blocks file processing before writing to disk
        var authResult = await _authService.AuthorizeAsync(User, rawSubmission, "MustOwnResource");
        if (!authResult.Succeeded) return Forbid();
    
        SubmissionFileResponseDTO? FileMetaData = await _service.SaveFileAsync(submissionid,request,cancellationToken);
        return Accepted(new { Message = "File uploaded successfully. Tracking Id for Async work is generated. ", TrackingIdentifier = FileMetaData.TrackingIdentifier , FileMetaData });
    }

        [HttpGet]
        [Route("{submissionid}/summary")]
        public async Task<ActionResult<SubmissionSummaryDTO>> GetSubmissionSummary([FromRoute]int submissionid, CancellationToken cancellationToken)
        {
            var rawSubmission = await _service.GetRawByIdAsync(submissionid);
            if (rawSubmission is null) return NotFound();

            // Centralized guard blocks entry before reading the memory cache
            var authResult = await _authService.AuthorizeAsync(User, rawSubmission, "MustOwnResource");
            if (!authResult.Succeeded) return Forbid();

            var summary =  await _service.GetSubmissionSummaryAsync(submissionid,cancellationToken);
            return Ok(summary);
        }
}
