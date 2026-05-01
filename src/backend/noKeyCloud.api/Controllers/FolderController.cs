using MediatR;
using Microsoft.AspNetCore.Mvc;


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
}

