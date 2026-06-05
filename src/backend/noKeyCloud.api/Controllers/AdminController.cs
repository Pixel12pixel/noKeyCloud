using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using noKeyCloud.Application.Features.Admin.GenerateRegisterInvite;
using noKeyCloud.Application.Features.Admin.GetActiveRegisterInvites;
using noKeyCloud.Application.Features.Admin.RevokeRegisterInvite;
using noKeyCloud.Contracts.Admin;

namespace noKeyCloud.api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin")] 
public class AdminController(IMediator mediator) : ControllerBase
{
    [HttpPost("generate-register-invite")]
    public async Task<IActionResult> GenerateInvite([FromBody] GenerateRegisterInviteRequest request)
    {
        var command = new GenerateRegisterInviteCommand(request.ExpirationHours);
        var result = await mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        
        return BadRequest(result.Error);
    }
    
    [HttpGet("active-register-invites")]
    public async Task<IActionResult> GetInvites()
    {
        var result = await mediator.Send(new GetActiveRegisterInvitesQuery());
        return Ok(result);
    }

    [HttpDelete("revoke-invite/{id}")]
    public async Task<IActionResult> RevokeInvite(Guid id)
    {
        var result = await mediator.Send(new RevokeRegisterInviteCommand(id));
        if (result.IsSuccess) return Ok();
        
        return BadRequest(result.Error);
    }
}