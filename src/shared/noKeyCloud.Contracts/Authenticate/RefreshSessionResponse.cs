namespace noKeyCloud.Contracts.Authenticate;

public record RefreshSessionResponse(string RefreshToken, DateTime ExpiresAt, string JwtToken);