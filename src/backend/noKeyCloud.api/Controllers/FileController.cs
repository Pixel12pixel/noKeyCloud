using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using noKeyCloud.Application.Features.Files.CreateFile;
using noKeyCloud.Contracts.File;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace noKeyCloud.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileController : ControllerBase
{
    private readonly IMediator _mediator;

    public FileController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [HttpPost("createFile")]
    public async Task<IActionResult> CreateFile([FromBody] CreateFileRequest request)
    {
        var userIdClaim = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("Invalid token claims");
        }


        var command = new CreateFileCommand(
            userId,
            request.FileName,
            request.FolderId);

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok();
        }
        return BadRequest(result.Error);
    }
}