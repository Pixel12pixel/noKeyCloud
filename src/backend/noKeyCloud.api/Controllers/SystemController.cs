using MediatR;
using Microsoft.AspNetCore.Mvc;
using noKeyCloud.Application.Features.System.GetSetupStatus;

namespace noKeyCloud.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SystemController(IMediator mediator) : ControllerBase
{
    [HttpGet("setup-status")]
    public async Task<IActionResult> GetStatus()
    {
        var needsSetup = await mediator.Send(new GetSetupStatusQuery());
        return Ok(new { NeedsSetup = needsSetup });
    }
}