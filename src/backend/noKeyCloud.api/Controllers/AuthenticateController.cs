using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using noKeyCloud.Application.Features.Users.LoginInit;
using noKeyCloud.Application.Features.Users.LoginVerify;
using noKeyCloud.Application.Features.Users.Logout;
using noKeyCloud.Application.Features.Users.RefreshSession;
using noKeyCloud.Application.Features.Users.Register;
using noKeyCloud.Application.Features.Users.RemoveUser;
using noKeyCloud.Contracts.Authenticate;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshSession([FromBody] RefreshSessionRequest request)

    {
        var userId = Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub));

        var command = new RefreshSessionCommand(userId, request.RefreshToken);
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            return Unauthorized(result.Error);
        }

        return Ok(result.Value);
    }


    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveUser([FromRoute] Guid id, RemoveUserRequest request)
    {
        var command = new RemoveUserCommand(
            id);

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return BadRequest(result.Error);
    }


    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] LogoutUserRequest request)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("User context is invalid.");
        }

        var command = new LogoutUserCommand(userId, request.RefreshToken);

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        if (result.Error.Contains("Token.NotFound"))
        {
            return NotFound(result.Error);
        }

        return BadRequest(result.Error);
    }
}