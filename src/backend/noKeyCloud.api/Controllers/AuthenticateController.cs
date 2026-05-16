using MediatR;
using Microsoft.AspNetCore.Mvc;
using noKeyCloud.Application.Features.Users.Register;
using noKeyCloud.Contracts.Authenticate;
using noKeyCloud.Application.Features.Users.LoginInit;
using noKeyCloud.Application.Features.Users.LoginVerify;
using noKeyCloud.Application.Abstractions.Services;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

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
    
    [HttpPost("login/srp/init")]
    public async Task<IActionResult> InitLogin([FromBody] LoginInitRequest request)
    {
        var command = new LoginInitCommand(
            request.Username,
            request.Email,
            request.A);

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
    
        return BadRequest(result.Error);
    }

    [HttpPost("login/srp/verify")]
    public async Task<IActionResult> VerifyLogin([FromBody] LoginVerifyRequest request)
    {
        var command = new LoginVerifyCommand(
            request.SessionId,
            request.M1);

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {

           return Ok(result.Value);
        }
        else if (result.Error == "Invalid credentials" || result.Error == "Session not found." || result.Error == "SRP verification failed")
        {
            return Unauthorized(result.Error);
        }

        return BadRequest(result.Error);
    }
}