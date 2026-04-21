using MediatR;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Contracts.Common;
using noKeyCloud.Domain.Entities;

namespace noKeyCloud.Application.Features.Users.Register;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Result>
{
    
    private readonly IUserRepository _userRepository;
    
    public RegisterUserHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<Result> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = new User(Guid.NewGuid(), request.Email, request.Username, request.Salt, request.Verifier);
            
            await _userRepository.CreateUser(user);
        }
        catch (Exception e)
        {
            return Result.Failure(e.Message);
        }
        
        return Result.Success();
    }
}