using noKeyCloud.Contracts.Authenticate;

namespace noKeyCloud.Application.Features.Users.LoginVerify;

public record LoginVerifyResult(
    LoginVerifyResponse ResponsePayload, 
    string JwtToken, 
    string RefreshToken);