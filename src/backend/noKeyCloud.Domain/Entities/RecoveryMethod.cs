using noKeyCloud.Domain.Enums;

namespace noKeyCloud.Domain.Entities;

public class RecoveryMethod
{
    
    public Guid Id { get; private set; }
    
    public RecoveryMethodType RecoveryMethodType { get; private set; }
    
    public byte[] EncryptedRecoveryData { get; private set; }
    
    public bool IsActive { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    
    
    public Guid UserId { get; private set; }
    
    //Navigation
    
    public User User { get; private set; }
    
    //Validation
    
    public RecoveryMethod(Guid id, RecoveryMethodType recoveryMethodType, byte[] encryptedRecoveryData, Guid userId)
    {
        Id = id;
        RecoveryMethodType = recoveryMethodType;
        EncryptedRecoveryData = encryptedRecoveryData;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UserId = userId;
    }
    
    //EF
    private RecoveryMethod()
    {
    }
    
}