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
    Task SaveSessionAsync(Guid sessionId, Guid userId, SrpSession session);
    Task<SrpSession?> GetSessionAsync(Guid sessionId);
    Task<Guid?> GetUserIdAsync(Guid sessionId);
    Task<bool> DeleteSessionAsync(Guid sessionId);
}