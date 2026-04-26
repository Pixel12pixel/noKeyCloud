using MediatR;
using Microsoft.AspNetCore.Mvc;
using noKeyCloud.Application.Features.Users.CreateFile;
using noKeyCloud.Contracts.Authenticate;

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
    
    [HttpPost("createFile")]
    public async Task<IActionResult> CreateFile([FromBody] CreateFileRequest request)
    {
        var command = new CreateFileCommand(
            request.UserId,
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