namespace noKeyCloud.Domain.Entities;

public class Session
{
    
    public Guid Id { get; private set; }
    
    public string TokenHash { get; private set; }
    
    public string DeviceNametag { get; private set; }
    
    public string IpAddress { get; private set; }
    
    public string UserAgent { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    
    public DateTime ExpiresAt { get; private set; }
    
    
    public Guid UserId { get; private set; }
    
    //Navigation
    
    public User User { get; private set; }
    
    //Validation
    
    public Session(Guid id, string tokenHash, string deviceNametag, string ipAddress, string userAgent, 
        DateTime expiresAt, Guid userId)
    {
        Id = id;
        TokenHash = tokenHash;
        DeviceNametag = deviceNametag;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = expiresAt;
        UserId = userId;
    }
    
    //EF
    private  Session()
    {
    }
    
}