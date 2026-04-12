using noKeyCloud_api.DTOs.Auth;
using noKeyCloud_api.Repositories.Interfaces;
using noKeyCloud_api.Services.Interfaces;
using noKeyCloud_api.Data.Models;

namespace noKeyCloud_api.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    
    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<string> CreateUser(RegisterDTO request)
    {
        AuthCredentials credential = new AuthCredentials()
        {
            Id = Guid.NewGuid(),
            Salt = request.Salt!,
            Verifier = request.Verifier!,
            CreatedAt = DateTime.UtcNow,
        };
        
        Users user = new Users()
        {
            Id = Guid.NewGuid(),
            Email =  request.Email!,
            Username = request.Username,
            IsActive = true,
            CreatedAt =  DateTime.UtcNow,
            Credentials =  credential
        };
        
        var result = await _userRepository.CreateUser(user);
        return result.ToString();
    }
}