using Microsoft.Extensions.Caching.Distributed;

public class CachedRefreshTokenProvider(IDistributedCache cache) : ICachedRefreshTokenProvider
{
    public async Task CachedRefreshTokenProviderAsync(Guid userId, string refreshToken, DistributedCacheEntryOptions options, CancellationToken cancellationToken)
    {
        await cache.SetStringAsync($"RefreshToken_{userId}", refreshToken, options, cancellationToken);
        await cache.SetStringAsync($"TokenUser_{refreshToken}", userId.ToString(), options, cancellationToken);
    }
}
