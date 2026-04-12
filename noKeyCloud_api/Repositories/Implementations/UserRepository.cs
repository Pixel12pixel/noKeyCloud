using noKeyCloud_api.Data.Context;
using noKeyCloud_api.Data.Models;
using noKeyCloud_api.DTOs.Auth;
using noKeyCloud_api.Repositories.Interfaces;

namespace noKeyCloud_api.Repositories.Implementations;

public class UserRepository : IUserRepository
{
    private readonly DataContext _context;
    
    public UserRepository(DataContext context)
    {
        _context = context;
    }
    
    public async Task<bool> CreateUser(Users user)
    {
        await _context.Users.AddAsync(user);
        return true;
    }
}