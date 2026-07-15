
namespace noKeyCloud.Contracts.Authenticate;

public class RegisterUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public required byte[] Salt { get; set; }
    public required byte[] Verifier { get; set; }
    
    public required byte[] EncryptedMasterKey { get; set; }
    
    public required byte[] KeySalt { get; set; }
    
    public required byte[] RecoveryEncryptedMasterKey { get; set; }
    
    public required byte[] RootFolderKey { get; set; }
    public string? RegisterInviteCode { get; set; }
}