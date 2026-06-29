using Microsoft.AspNetCore.Mvc;

namespace TrainingDirectory.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    
    [HttpGet]
    public ActionResult Get()
    {
        return Ok(
            new { status= "running", application= "Training Directory", timestamp= DateTime.Now}
        );
    }
}
