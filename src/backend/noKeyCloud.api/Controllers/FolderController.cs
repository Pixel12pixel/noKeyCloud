using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using noKeyCloud.api.Controllers.DTOs;
using noKeyCloud.Application.Features.Folders.CreateFolder;
using noKeyCloud.Application.Features.Folders.ListContent;
using noKeyCloud.Contracts.Folders;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using noKeyCloud.Application.Features.Folders;
using noKeyCloud.Application.Features.Folders.GetAncestry;


namespace noKeyCloud.api.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class FolderController : ControllerBase
{
    private readonly IMediator _mediator;
    public FolderController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [Authorize]
    [HttpGet("GetContent")]
    public async Task<IActionResult> GetContent([FromQuery] ListContentRequest listContentRequest, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("Invalid token claims");
        }

        var query = new ListContentQuery(listContentRequest.FolderId, userId);
        var result = await _mediator.Send(query, cancellationToken);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        return BadRequest(result.Error);
    }
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<CreateFolderResponse>> Create([FromBody] CreateFolderRequest request, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("Invalid token claims");
        }

        var command = new CreateFolderCommand(
            userId,
            request.EncryptedName,
            request.EncryptedKey,
            request.ParentFolderId);

        var response = await _mediator.Send(command, cancellationToken);

        return Created($"/api/Folder/{response.Id}", response);
    }
    
    [Authorize]
    [HttpGet("GetAncestry")]
    public async Task<ActionResult<CreateFolderResponse>> GetAncestry([FromQuery] GetAncestryRequest request, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("Invalid token claims");
        }

        var query = new GetAncestryQuery(userId, request.FolderId);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return NotFound(result.Error);
    }
}