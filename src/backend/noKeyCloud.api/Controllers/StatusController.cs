using Microsoft.AspNetCore.Mvc;

namespace noKeyCloud.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatusController : ControllerBase
{
    
    [HttpGet("ping")]
    public async Task<IActionResult> Ping()
    {
        return Ok("pong");
    }
    
    //TODO: Later add full system status endpoint
}