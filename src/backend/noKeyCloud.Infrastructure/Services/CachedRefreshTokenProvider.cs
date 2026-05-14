using System.Security.Cryptography;
using Microsoft.Extensions.Caching.Distributed;
using noKeyCloud.Application.Abstractions.Services;

namespace noKeyCloud.Infrastructure.Services;

public class CachedRefreshTokenProvider(IDistributedCache cache) : IRefreshTokenProvider
{
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public async Task<string?> GetRefreshTokenAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await cache.GetStringAsync($"RefreshToken_{userId}", cancellationToken);
    }

    public async Task StoreRefreshTokenAsync(Guid userId, string refreshToken, TimeSpan expiry, CancellationToken cancellationToken = default)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiry
        };

        await cache.SetStringAsync($"RefreshToken_{userId}", refreshToken, options, cancellationToken);
    }

    public async Task InvalidateRefreshTokenAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await cache.RemoveAsync($"RefreshToken_{userId}", cancellationToken);
    }
}