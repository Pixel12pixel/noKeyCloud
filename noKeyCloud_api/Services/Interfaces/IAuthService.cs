using noKeyCloud_api.DTOs.Auth;

namespace noKeyCloud_api.Services.Interfaces;

public interface IAuthService
{
    Task<String> CreateUser(RegisterDTO request);
}