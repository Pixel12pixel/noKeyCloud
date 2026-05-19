namespace noKeyCloud.Contracts.Authenticate;

public record RefreshSessionRequest(string RefreshToken, Guid UserId);