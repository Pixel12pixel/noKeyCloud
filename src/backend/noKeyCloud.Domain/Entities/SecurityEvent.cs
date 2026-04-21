using noKeyCloud.Domain.Enums;

namespace noKeyCloud.Domain.Entities;

public class SecurityEvent
{
    public Guid Id { get; private set; }
    
    public SecurityEventType Type { get; private set; }
    
    public string Details { get; private set; }
    
    public string IpAddress { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    
    
    public Guid UserId { get; private set; }
    
    //Navigation
    
    public User User { get; private set; }
    
    //Validation
    
    public SecurityEvent(Guid id, SecurityEventType type, string details, string ipAddress, Guid userId)
    {
        Id = id;
        Type = type;
        Details = details;
        IpAddress = ipAddress;
        CreatedAt = DateTime.UtcNow;
        UserId = userId;
    }
    
    //EF
    
    private  SecurityEvent()
    {
    }
}