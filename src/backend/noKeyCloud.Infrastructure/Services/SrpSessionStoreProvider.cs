using System.Collections.Concurrent;
using System.Text.Json;
using noKeyCloud.Application.Abstractions.Services;
using StackExchange.Redis;

namespace noKeyCloud.Infrastructure.Services;

public class SrpSessionStoreProvider : ISrpSessionStore
{
    private readonly IDatabase _database;

    public SrpSessionStoreProvider(IConnectionMultiplexer redis)
    {
        _database = redis.GetDatabase();
    }

    public async Task SaveSessionAsync(Guid sessionId, Guid userId, SrpSession session)
    {
        var sessionJson = JsonSerializer.Serialize(session);
        
        var sessionKey = $"srp:session:{sessionId}";
        var userKey = $"srp:user:{sessionId}";

        var timeSpan = TimeSpan.FromMinutes(5);
        
        await _database.StringSetAsync(sessionKey, sessionJson, timeSpan);
        
        await _database.StringSetAsync(userKey, userId.ToString(), timeSpan);
    }

    public async Task<SrpSession?> GetSessionAsync(Guid sessionId)
    {
        var sessionKey = $"srp:session:{sessionId}";
        
        var value = await _database.StringGetAsync(sessionKey);
        
        if (value.IsNullOrEmpty) return null;
        
        return JsonSerializer.Deserialize<SrpSession>(value.ToString());
    }

    public async Task<Guid?> GetUserIdAsync(Guid sessionId)
    {
        var userKey = $"srp:user:{sessionId}";
        
        var value = await _database.StringGetAsync(userKey);
        
        if (value.IsNullOrEmpty) return null;
        
        return Guid.Parse(value.ToString());
    }

    public async Task<bool> DeleteSessionAsync(Guid sessionId)
    {
        var sessionKey = $"srp:session:{sessionId}";
        var userKey = $"srp:user:{sessionId}";
        
        var deleted = await _database.KeyDeleteAsync(new RedisKey[] { sessionKey, userKey });

        return deleted == 2;
    }
}