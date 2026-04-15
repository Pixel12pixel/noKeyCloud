using MediatR;
using Microsoft.AspNetCore.Mvc;
using noKeyCloud.Application.Features.Users.Register;
using noKeyCloud.Contracts.Authenticate;

namespace noKeyCloud.api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class AuthenticateController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public AuthenticateController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        var command = new RegisterUserCommand(
            request.Username,
            request.Email,
            request.Salt,
            request.Verifier);
        var result = await _mediator.Send(command);
        if (result.IsSuccess)
        {
            return Ok();
        }
        return BadRequest(result.Error);
    }
}