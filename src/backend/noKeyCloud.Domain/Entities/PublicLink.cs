namespace noKeyCloud.Domain.Entities;

public class PublicLink
{
    
    public Guid Id { get; private set; }
    
    public string ResourceType { get; private set; }
    
    public string PublicToken { get; private set; }
    
    public byte[] PasswordHash { get; private set; }
    
    public byte[] EncryptedLinkKey { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    
    public DateTime ExpiresAt { get; private set; }
    
    
    public Guid UserId { get; private set; }
    
    public Guid? ResourceFolderId { get; private set; }
    
    public Guid? ResourceFileId { get; private set; }
    
    
    //Navigation
    
    public User User { get; private set; }
    
    public Folder? Folder { get; private set; }
    
    public File? File { get; private set; }
    
    //Validation
    
    public PublicLink(Guid id, string resourceType, string publicToken, byte[] passwordHash, byte[] encryptedLinkKey, DateTime expiresAt,
        Guid userId, Guid? resourceFolderId, Guid? resourceFileId)
    {
        Id = id;
        ResourceType = resourceType;
        PublicToken = publicToken;
        PasswordHash = passwordHash;
        EncryptedLinkKey = encryptedLinkKey;
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = expiresAt;
        UserId = userId;
        ResourceFolderId = resourceFolderId;
        ResourceFileId = resourceFileId;
    }
    
    //EF
    private PublicLink()
    {}
}