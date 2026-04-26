namespace noKeyCloud.Domain.Entities;

public class File
{
    
    public Guid Id { get; private set; }
    
    public byte[] EncryptedName { get; private set; }
    
    public string MimeType { get; private set; }
    
    public long SizeBytes { get; private set; }
    
    public string StoragePath { get; private set; }
    
    public byte[] EncryptedKey { get; private set; }
    
    public byte[] Checksum { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    
    public DateTime UpdatedAt { get; private set; }
    
    
    public Guid ParentFolderId { get; private set; }
    
    public Guid UserId { get; private set; }
    
    
    //Navigation
    
    public Folder ParentFolder { get; private set; }
    
    public User OwnerUser { get; private set; }
    
    //Validation
    
    public File(Guid id, byte[] encryptedName, string mimeType, long sizeBytes, string storagePath, byte[] encryptedKey, byte[] checksum,
        Guid parentFolderId, Guid userId,  Folder parentFolder, User user)
    {
        Id = id;
        EncryptedName = encryptedName;
        MimeType = mimeType;
        SizeBytes = sizeBytes;
        StoragePath = storagePath;
        EncryptedKey = encryptedKey;
        Checksum = checksum;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
        ParentFolderId = parentFolderId;
        UserId = userId;
        ParentFolder = parentFolder;
        OwnerUser = user;
    }
    
    //EF
    private File()
    {
    }
}