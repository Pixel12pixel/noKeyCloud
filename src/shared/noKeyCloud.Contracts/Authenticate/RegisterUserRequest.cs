
namespace noKeyCloud.Contracts.Authenticate;

public class RegisterUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public required byte[] Salt { get; set; }
    public required byte[] Verifier { get; set; }
    public string? RegisterInviteCode { get; set; }
}