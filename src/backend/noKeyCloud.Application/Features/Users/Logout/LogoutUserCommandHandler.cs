using MediatR;
using noKeyCloud.Application.Abstractions.Services;
using noKeyCloud.Contracts.Authenticate;
using noKeyCloud.Contracts.Common;

namespace noKeyCloud.Application.Features.Users.Logout;

public class LogoutUserCommandHandler(IRefreshTokenProvider refreshTokenProvider)
    : IRequestHandler<LogoutUserCommand, Result<LogoutUserResponse>>
{
    public async Task<Result<LogoutUserResponse>> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
    {
        var existingToken = await refreshTokenProvider.GetRefreshTokenAsync(request.UserId, cancellationToken);
        
        if (string.IsNullOrEmpty(existingToken) || existingToken != request.RefreshToken)
        {
            return Result<LogoutUserResponse>.Failure("Token.NotFound: Token is invalid or already removed.");
        }
        
        await refreshTokenProvider.InvalidateRefreshTokenAsync(request.UserId, cancellationToken);
        
        return Result<LogoutUserResponse>.Success(new LogoutUserResponse("User logged out successfully."));
    }
}