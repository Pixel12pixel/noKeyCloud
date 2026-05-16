namespace noKeyCloud.Contracts.Authenticate;

public record RefreshSessionRequest(Guid UserId, string RefreshToken);