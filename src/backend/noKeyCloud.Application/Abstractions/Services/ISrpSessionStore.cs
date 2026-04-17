using Org.BouncyCastle.Crypto.Agreement.Srp;

namespace noKeyCloud.Application.Abstractions.Services;

public interface ISrpSessionStore
{
    void SaveSession(Guid sessionId, Srp6Server srpServer);
    Srp6Server? GetSession(Guid sessionId);
}