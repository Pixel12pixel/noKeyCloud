using MediatR;
using noKeyCloud.Application.Abstractions.Services;
using noKeyCloud.Contracts.Authenticate;
using noKeyCloud.Contracts.Common;

namespace noKeyCloud.Application.Features.Users.RefreshSession;

public class RefreshSessionHandler(IJwtService jwtService, IRefreshTokenProvider refreshTokenProvider)
    : IRequestHandler<RefreshSessionCommand, Result<RefreshSessionResult>>
{
    public async Task<Result<RefreshSessionResult>> Handle(RefreshSessionCommand request, CancellationToken cancellationToken)
    {
        var storedToken = await refreshTokenProvider.GetRefreshTokenAsync(request.UserId, cancellationToken);
        
        if (string.IsNullOrEmpty(storedToken) || storedToken != request.RefreshToken)
        {
            return Result<RefreshSessionResult>.Failure("Invalid or expired refresh token.");
        }
        
        var newRefreshToken = refreshTokenProvider.GenerateRefreshToken();
        
        await refreshTokenProvider.StoreRefreshTokenAsync(request.UserId, newRefreshToken, TimeSpan.FromHours(24), cancellationToken);
        var newJwt = await jwtService.JwtTokenService(request.UserId);
        
        var responsePayload = new RefreshSessionResponse(DateTime.UtcNow.AddMinutes(15));
        
        var applicationResult = new RefreshSessionResult(responsePayload, newJwt, newRefreshToken);
        
        return Result<RefreshSessionResult>.Success(applicationResult);
    }
}