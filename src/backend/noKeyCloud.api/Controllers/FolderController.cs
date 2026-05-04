using MediatR;
using Microsoft.AspNetCore.Mvc;
using noKeyCloud.Application.Features.Folders.ListContent;
using noKeyCloud.Contracts.Folders;


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
    public async Task<IActionResult> GetContent([FromBody] ListContentRequest listContentRequest, CancellationToken cancellationToken)
    {
        var query = new ListContentQuery(listContentRequest.FolderId, listContentRequest.FolderId);
        var result = await _mediator.Send(query, cancellationToken);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        return BadRequest(result.Error);
    }
}

