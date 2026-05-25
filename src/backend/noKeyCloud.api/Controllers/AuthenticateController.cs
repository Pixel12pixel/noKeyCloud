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
            Response.Cookies.Append("access_token", result.Value!.JwtToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/",
                Expires = DateTime.UtcNow.AddMinutes(15)
            });
            
            Response.Cookies.Append("refresh_token", result.Value!.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, 
                SameSite = SameSiteMode.Strict,
                Path = "/api/Authenticate/refresh",
                Expires = DateTime.UtcNow.AddHours(24)
            });
            
            return Ok(result.Value!.ResponsePayload);
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
        if (!Request.Cookies.TryGetValue("refresh_token", out var refreshTokenCookie) || string.IsNullOrEmpty(refreshTokenCookie))
        {
            return Unauthorized("Refresh token is missing.");
        }
        
        var command = new RefreshSessionCommand(request.UserId, refreshTokenCookie);
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            return Unauthorized(result.Error);
        }
        
        Response.Cookies.Append("access_token", result.Value!.JwtToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/",
            Expires = DateTime.UtcNow.AddMinutes(15)
        });
        
        Response.Cookies.Append("refresh_token", result.Value!.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/api/Authenticate/refresh",
            Expires = DateTime.UtcNow.AddHours(24)
        });

        return Ok(result.Value!.ResponsePayload);
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
    public async Task<IActionResult> Logout()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("User context is invalid.");
        }
        
        Request.Cookies.TryGetValue("refresh_token", out var refreshTokenCookie);

        var command = new LogoutUserCommand(userId, refreshTokenCookie ?? "");

        var result = await _mediator.Send(command);
        
        Response.Cookies.Delete("access_token", new CookieOptions { Path = "/" });
        Response.Cookies.Delete("refresh_token", new CookieOptions { Path = "/api/Authenticate/refresh" });

        if (result.IsSuccess)
        {
            return Ok();
        }

        if (result.Error != null && result.Error.Contains("Token.NotFound"))
        {
            return Ok(); 
        }

        return BadRequest(result.Error);
    }
}