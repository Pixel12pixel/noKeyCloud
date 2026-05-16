namespace noKeyCloud.Application.Abstractions.Services;

public interface IRefreshTokenProvider
{
    Task StoreRefreshTokenAsync(Guid userId, string refreshToken, TimeSpan expiry, CancellationToken cancellationToken = default);
    Task<string?> GetRefreshTokenAsync(Guid userId, CancellationToken cancellationToken = default);
    Task InvalidateRefreshTokenAsync(Guid userId, CancellationToken cancellationToken = default);
    string GenerateRefreshToken();
}