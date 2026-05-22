using System.Numerics;

namespace noKeyCloud.Application.Abstractions.Services;

public class SrpSession
{
    public string Username { get; set; } = string.Empty;
    public byte[] Salt { get; set; } = Array.Empty<byte>();
    public BigInteger A { get; set; }
    public BigInteger B { get; set; }
    public BigInteger S { get; set; }
}

public interface ISrpSessionStore
{
    void SaveSession(Guid sessionId, Guid userId, SrpSession session);
    SrpSession? GetSession(Guid sessionId);
    Guid? GetUserId(Guid sessionId);
    bool DeleteSession(Guid sessionId);
}