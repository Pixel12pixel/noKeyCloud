namespace noKeyCloud.Domain.Entities;

public class RegisterInvite(Guid id, string code, DateTime createdAt, DateTime? expiresAt)
{
    public Guid Id { get; private set; } = id;
    public string Code { get; private set; } = code;
    public bool IsUsed { get; private set; } = false;
    public DateTime CreatedAt { get; private set; } = createdAt;
    public DateTime? ExpiresAt { get; private set; } = expiresAt;

    public void MarkAsUsed()
    {
        IsUsed = true;
    }
    
    public bool IsValid()
    {
        if (IsUsed) return false;
        if (ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow) return false;
        return true;
    }
}