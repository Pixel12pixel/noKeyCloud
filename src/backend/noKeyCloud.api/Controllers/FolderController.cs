using MediatR;
using Microsoft.AspNetCore.Mvc;
using noKeyCloud.api.Controllers.DTOs;
using noKeyCloud.Application.Features.Folders.Commands.CreateFolder;

namespace noKeyCloud.api.Controllers;

[ApiController]
[Route("api/[controller]")]
// TODO: Re-enable [Authorize] once authentication middleware and JWT generation are fully implemented.
// [Authorize] 
public class FolderController(IMediator mediator) : ControllerBase
{
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