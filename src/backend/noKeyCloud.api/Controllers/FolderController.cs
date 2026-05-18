using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using noKeyCloud.api.Controllers.DTOs;
using noKeyCloud.Application.Features.Folders.CreateFolder;
using noKeyCloud.Application.Features.Folders.ListContent;
using noKeyCloud.Contracts.Folders;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


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
        var userId = Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub));

        var query = new ListContentQuery(listContentRequest.FolderId, userId);
        var result = await _mediator.Send(query, cancellationToken);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        return BadRequest(result.Error);
    }
    [Authorize]
    [HttpPost("AddFolder")]
    public async Task<ActionResult<CreateFolderResponse>> Create([FromBody] CreateFolderRequest request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub));

        var command = new CreateFolderCommand(
            userId,
            request.Name,
            request.ParentFolderId);

        var response = await _mediator.Send(command, cancellationToken);

        return Created($"/api/Folder/{response.Id}", response);
    }
}