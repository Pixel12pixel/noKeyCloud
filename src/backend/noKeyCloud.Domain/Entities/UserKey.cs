namespace noKeyCloud.Domain.Entities;

public class UserKey
{
    //
    
    public Guid Id { get; private set; }
    
    public byte[] PublicKey { get; private set; }
    
    public byte[] EncryptedPrivateKey { get; private set; }
    
    public string KeyVersion { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    
    public Guid UserId { get; private set; }
    
    //Navigation
    
    public User User { get; private set; }
    
    //Validation
    
    public UserKey(Guid id, byte[] publicKey, byte[] encryptedPrivateKey, string keyVersion, Guid userId)
    {
        Id = id;
        PublicKey = publicKey;
        EncryptedPrivateKey = encryptedPrivateKey;
        KeyVersion = keyVersion;
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
    }
    
    //EF
    private UserKey()
    {
    }
}