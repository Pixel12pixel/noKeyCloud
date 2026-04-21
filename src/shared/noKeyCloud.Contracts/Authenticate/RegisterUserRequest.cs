
namespace noKeyCloud.Contracts.Authenticate;

public class RegisterUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public byte[] Salt { get; set; }
    public byte[] Verifier { get; set; }
}