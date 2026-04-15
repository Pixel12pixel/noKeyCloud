namespace noKeyCloud.Domain.Entities;

public class UserShare
{
    
    public Guid Id { get; private set; }
    
    public string ResourceType { get; private set; }
    
    public int PermissionLevel { get; private set; }
    
    public byte[] EncryptedSharedKey { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    
    public DateTime ExpiresAt { get; private set; }
    
    
    public Guid OwnerUserId { get; private set; }
    
    public Guid RecipientUserId { get; private set; }
    
    public Guid? ResourceFolderId { get; private set; }
    
    public Guid? ResourceFileId { get; private set; }
    
    //Navigation
    
    public User Owner { get; private set; }
    
    public User Recipient { get; private set; }
    
    public Folder? ResourceFolder { get; private set; }
    
    public File? ResourceFile { get; private set; }
    
    //Validation
    
    public UserShare(Guid id, string resourceType, int permissionLevel, byte[] encryptedSharedKey, DateTime expiresAt, 
        Guid ownerUserId, Guid recipientUserId, Guid? resourceFolderId, Guid? resourceFileId)
    {
        Id = id;
        ResourceType = resourceType;
        PermissionLevel = permissionLevel;
        EncryptedSharedKey = encryptedSharedKey;
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = expiresAt;
        OwnerUserId = ownerUserId;
        RecipientUserId = recipientUserId;
        ResourceFileId = resourceFileId;
        ResourceFolderId = resourceFolderId;
    }
    
    //EF
    private  UserShare()
    {
    }
    
}