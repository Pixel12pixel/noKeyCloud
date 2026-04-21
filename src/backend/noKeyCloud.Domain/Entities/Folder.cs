namespace noKeyCloud.Domain.Entities;

public class Folder
{
    
    public Guid Id { get; private set; }
    
    public byte[] EncryptedName { get; private set; }
    
    public byte[] EncryptedKey { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    
    public DateTime UpdatedAt { get; private set; }
    
    
    public Guid ParentFolderId { get; private set; }
    
    public Guid UserId { get; private set; }
    
    
    //Navigation
    
    public User User { get; private set; }
    
    public Folder ParentFolder { get; private set; }
    
    private readonly List<Folder> _subFolders = new();
    public IReadOnlyCollection<Folder> SubFolders => _subFolders.AsReadOnly();
    
    private readonly List<File> _files = new();
    public IReadOnlyCollection<File> Files => _files.AsReadOnly();
    
    //Validation

    public Folder(Guid id, byte[] encryptedName, byte[] encryptedKey, DateTime createdAt, DateTime updatedAt,
        Guid parentFolderId, Guid userId)
    {
        Id = id;
        EncryptedName = encryptedName;
        EncryptedKey = encryptedKey;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        ParentFolderId = parentFolderId;
        UserId = userId;
    }
    
    //EF
    private Folder()
    {
    }
}