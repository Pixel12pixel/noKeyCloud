using System.Collections.Concurrent;
using noKeyCloud.Application.Abstractions.Services;
using Org.BouncyCastle.Crypto.Agreement.Srp;

namespace noKeyCloud.Infrastructure.Services;

public class InMemorySrpSessionStore : ISrpSessionStore
{
    private readonly ConcurrentDictionary<Guid, Srp6Server> _sessions = new();

    public void SaveSession(Guid sessionId, Srp6Server srpServer)
    {
        _sessions[sessionId] = srpServer;
    }

    public Srp6Server? GetSession(Guid sessionId)
    {
        _sessions.TryGetValue(sessionId, out var session);
        return session;
    }

    public bool DeleteSession(Guid sessionId)
    {
        return _sessions.TryRemove(sessionId, out _);
    }
}