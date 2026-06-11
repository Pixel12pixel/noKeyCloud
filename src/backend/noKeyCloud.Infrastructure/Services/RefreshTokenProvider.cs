using Microsoft.Extensions.Caching.Distributed;
using noKeyCloud.Application.Abstractions.Services;
using System.Security.Cryptography;

namespace noKeyCloud.Infrastructure.Services;

public class RefreshTokenProvider(IDistributedCache cache, ICachedRefreshTokenProvider cachedRefreshTokenProvider) : IRefreshTokenProvider
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

    public async Task<Guid?> GetUserIdByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var userIdString = await cache.GetStringAsync($"TokenUser_{refreshToken}", cancellationToken);
        if (Guid.TryParse(userIdString, out var userId))
        {
            return userId;
        }
        return null;
    }

    public async Task StoreRefreshTokenAsync(Guid userId, string refreshToken, TimeSpan expiry, CancellationToken cancellationToken = default)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiry
        };

        var oldToken = await GetRefreshTokenAsync(userId, cancellationToken);
        if (!string.IsNullOrEmpty(oldToken))
        {
            await cache.RemoveAsync($"TokenUser_{oldToken}", cancellationToken);
        }

        await cachedRefreshTokenProvider.CachedRefreshTokenProviderAsync(userId, refreshToken, options, cancellationToken);
    }

    public async Task InvalidateRefreshTokenAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var oldToken = await GetRefreshTokenAsync(userId, cancellationToken);
        if (!string.IsNullOrEmpty(oldToken))
        {
            await cache.RemoveAsync($"TokenUser_{oldToken}", cancellationToken);
        }

        await cache.RemoveAsync($"RefreshToken_{userId}", cancellationToken);
    }

    public async Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = await GetUserIdByRefreshTokenAsync(refreshToken, cancellationToken);
        if (userId != null)
        {
            await cache.RemoveAsync($"RefreshToken_{userId.Value}", cancellationToken);
            await cache.RemoveAsync($"TokenUser_{refreshToken}", cancellationToken);
        }
    }
}