using MediatR;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Application.Abstractions.Services;
using noKeyCloud.Contracts.Authenticate;
using noKeyCloud.Contracts.Common;

namespace noKeyCloud.Application.Features.Users.RemoveUser;

public class RemoveUserCommandHandler(IUserRepository userRepository, IRefreshTokenProvider refreshTokenProvider)
    : IRequestHandler<RemoveUserCommand, Result<RemoveUserResponse>>
{
    public async Task<Result<RemoveUserResponse>> Handle(RemoveUserCommand request, CancellationToken cancellationToken)
    {
        var userId = request.Id;
        var user = await userRepository.GetUserByUserId(userId, cancellationToken);
        
        if (user == null) return Result<RemoveUserResponse>.Failure("User not found");

        var result = await userRepository.RemoveUserByUser(user, cancellationToken);
        
        if (!result.IsSuccess) return Result<RemoveUserResponse>.Failure(result.Error);

        await refreshTokenProvider.InvalidateRefreshTokenAsync(userId, cancellationToken);
        
        var response = new RemoveUserResponse("User successfully removed");

        return Result<RemoveUserResponse>.Success(response);
    }
}