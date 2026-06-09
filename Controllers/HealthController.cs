
using Microsoft.AspNetCore.Mvc;

namespace TraineeManagementApi.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    

    [HttpGet]
    public ActionResult Get()
    {
        return Ok(
            new { status= "running", application= "Trainee Management", timestamp= DateTime.Now}
        );
    }
}
