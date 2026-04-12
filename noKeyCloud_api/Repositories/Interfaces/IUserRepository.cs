using noKeyCloud_api.Data.Models;
using noKeyCloud_api.DTOs.Auth;

namespace noKeyCloud_api.Repositories.Interfaces;

public interface IUserRepository
{
    Task<bool> CreateUser(Users user);
}