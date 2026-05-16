using MediatR;
using noKeyCloud.Application.Abstractions.Services;
using noKeyCloud.Contracts.Authenticate;
using noKeyCloud.Contracts.Common;

namespace noKeyCloud.Application.Features.Users.RefreshSession;

public class RefreshSessionHandler(IJwtService jwtService, IRefreshTokenProvider refreshTokenProvider)
    : IRequestHandler<RefreshSessionCommand, Result<RefreshSessionResponse>>
{
    public async Task<Result<RefreshSessionResponse>> Handle(RefreshSessionCommand request, CancellationToken cancellationToken)
    {
        var storedToken = await refreshTokenProvider.GetRefreshTokenAsync(request.UserId, cancellationToken);
        
        if (string.IsNullOrEmpty(storedToken) || storedToken != request.RefreshToken)
        {
            return Result<RefreshSessionResponse>.Failure("Invalid or expired refresh token.");
        }
        
        var newRefreshToken = refreshTokenProvider.GenerateRefreshToken();
        var expiresAt = DateTime.UtcNow.AddHours(24);
        
        await refreshTokenProvider.StoreRefreshTokenAsync(request.UserId, newRefreshToken, TimeSpan.FromHours(24), cancellationToken);
        
        var newJwt = await jwtService.JwtTokenService(request.UserId);
        
        var response = new RefreshSessionResponse(newRefreshToken, expiresAt, newJwt);
        return Result<RefreshSessionResponse>.Success(response);
    }
}