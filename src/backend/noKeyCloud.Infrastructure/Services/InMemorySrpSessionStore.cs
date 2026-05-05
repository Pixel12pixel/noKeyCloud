using System.Collections.Concurrent;
using noKeyCloud.Application.Abstractions.Services;
using Org.BouncyCastle.Crypto.Agreement.Srp;

namespace noKeyCloud.Infrastructure.Services;

public class InMemorySrpSessionStore : ISrpSessionStore
{
    private readonly ConcurrentDictionary<Guid, Srp6Server> _sessions = new();
    private readonly ConcurrentDictionary<Guid, Guid> _userIds = new();

    public void SaveSession(Guid sessionId, Guid userId, Srp6Server srpServer)
    {
        _sessions[sessionId] = srpServer;
        _userIds[sessionId] = userId;
    }

    public Srp6Server? GetSession(Guid sessionId)
    {
        _sessions.TryGetValue(sessionId, out var session);
        return session;
    }

    public Guid? GetUserId(Guid sessionId)
    {
        if (_userIds.TryGetValue(sessionId, out var userId))
            return userId;
        return null;
    }

    public bool DeleteSession(Guid sessionId)
    {
        _userIds.TryRemove(sessionId, out _);
        return _sessions.TryRemove(sessionId, out _);
    }
}