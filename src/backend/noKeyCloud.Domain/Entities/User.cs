

using noKeyCloud.Contracts.Common;

namespace noKeyCloud.Domain.Entities;

public class User
{
    //
    
    public Guid Id { get; private set; }
    
    public string Email { get; private set; }
    
    public string Username { get; private set; }
    
    public bool IsActive { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    
    public DateTime UpdatedAt { get; private set; }
    
    public byte[] Salt { get; private set; }
    
    public byte[] Verifier { get; private set; }
    
    //Navigation
    
    private readonly List<UserKey> _userKeys = new();
    public IReadOnlyCollection<UserKey> UserKeys => _userKeys.AsReadOnly();
    
    private readonly List<Folder> _folders = new();
    public IReadOnlyCollection<Folder> Folders => _folders.AsReadOnly();
    
    private readonly List<File> _files = new();
    public IReadOnlyCollection<File> Files => _files.AsReadOnly();
    
    private readonly List<PublicLink> _publicLinks = new();
    public IReadOnlyCollection<PublicLink> PublicLinks => _publicLinks.AsReadOnly();
    
    private readonly List<UserShare> _userShares = new();
    public IReadOnlyCollection<UserShare> UserShares => _userShares.AsReadOnly();
    
    private readonly List<RecoveryMethod> _recoveryMethods = new();
    public IReadOnlyCollection<RecoveryMethod> RecoveryMethods => _recoveryMethods.AsReadOnly();
    
    private readonly List<Session> _sessions = new();
    public IReadOnlyCollection<Session> Sessions => _sessions.AsReadOnly();
    
    private readonly List<SecurityEvent> _securityEvents = new();
    public IReadOnlyCollection<SecurityEvent> SecurityEvents => _securityEvents.AsReadOnly();
    
    //Validation

    public User(Guid id, string email, string username, byte[] salt, byte[] verifier)
    {
        Id = id;
        Email = email;
        Username = username;
        IsActive = true;
        Salt = salt;
        Verifier = verifier;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public Result ChangePassword(byte[] salt, byte[] verifier)
    {
        if (salt == null || verifier == null)
        {
            return Result.Failure("Salt and verifier cannot be null.");
        }
        Salt = salt;
        Verifier = verifier;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }


    //EF
    private User()
    {
    }

}