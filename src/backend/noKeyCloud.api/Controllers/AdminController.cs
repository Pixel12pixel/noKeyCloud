using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using noKeyCloud.Application.Features.Admin.GenerateRegisterInvite;

namespace noKeyCloud.api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin")] 
public class AdminController(IMediator mediator) : ControllerBase
{
    [HttpPost("generate-register-invite")]
    public async Task<IActionResult> GenerateInvite()
    {
        var command = new GenerateRegisterInviteCommand(ExpirationDays: 7); 
        var result = await mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(new { code = result.Value });
        }
        
        return BadRequest(result.Error);
    }
}