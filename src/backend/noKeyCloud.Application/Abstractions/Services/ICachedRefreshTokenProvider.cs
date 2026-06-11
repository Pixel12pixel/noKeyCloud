using Microsoft.Extensions.Caching.Distributed;

public interface ICachedRefreshTokenProvider
{
    Task CachedRefreshTokenProviderAsync(Guid userId, string refreshToken, DistributedCacheEntryOptions options, CancellationToken cancellationToken);

}
