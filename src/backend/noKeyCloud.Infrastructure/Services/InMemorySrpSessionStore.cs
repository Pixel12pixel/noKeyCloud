using System.Collections.Concurrent;
using noKeyCloud.Application.Abstractions.Services;

namespace noKeyCloud.Infrastructure.Services;

public class InMemorySrpSessionStore : ISrpSessionStore
{
    private readonly ConcurrentDictionary<Guid, SrpSession> _sessions = new();
    private readonly ConcurrentDictionary<Guid, Guid> _userIds = new();

    public void SaveSession(Guid sessionId, Guid userId, SrpSession session)
    {
        _sessions[sessionId] = session;
        _userIds[sessionId] = userId;
    }

    public SrpSession? GetSession(Guid sessionId)
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