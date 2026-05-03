using MediatR;
using Microsoft.AspNetCore.Mvc;
using noKeyCloud.api.Controllers.DTOs;
using noKeyCloud.Application.Features.Folders.Commands.CreateFolder;

namespace noKeyCloud.api.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class FolderController : ControllerBase
{
    private readonly Mediator _mediator;
    public FolderController(Mediator mediator)
    {
        _mediator = mediator;
    }
    [HttpGet("GetContent")]
    public async Task<IActionResult> GetContent(Guid folderId, Guid userId, CancellationToken cancellationToken)
    {
        var query = new Application.Features.Folders.ListContent.ListContentQuery(folderId, userId);
        var result = await _mediator.Send(query, cancellationToken);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        return BadRequest(result.Error);
    }
    
    [HttpPost]
    public async Task<ActionResult<CreateFolderResponse>> Create([FromBody] CreateFolderRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateFolderCommand(
            request.UserId,
            request.Name,
            request.ParentFolderId);

        var response = await mediator.Send(command, cancellationToken);
        
        return Created($"/api/Folder/{response.Id}", response);
    }
}