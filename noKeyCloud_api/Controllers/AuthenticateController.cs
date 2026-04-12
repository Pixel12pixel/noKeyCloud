using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using noKeyCloud_api.Data.Models;
using noKeyCloud_api.DTOs.Auth;
using noKeyCloud_api.Repositories.Interfaces;
using noKeyCloud_api.Services.Interfaces;

namespace noKeyCloud_api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class AuthenticateController : ControllerBase
{
    private readonly IAuthService _authService;
    
    public AuthenticateController(IAuthService authService)
    {
        _authService = authService;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO request)
    {
        if (request is null) return BadRequest("Data can not be null");
        
        String result = await _authService.CreateUser(request);
        return Ok(result);
    }
}