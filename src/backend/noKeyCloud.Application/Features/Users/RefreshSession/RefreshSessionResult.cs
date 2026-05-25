using noKeyCloud.Contracts.Authenticate;

namespace noKeyCloud.Application.Features.Users.RefreshSession;

public record RefreshSessionResult(
    RefreshSessionResponse ResponsePayload, 
    string JwtToken, 
    string RefreshToken
    );