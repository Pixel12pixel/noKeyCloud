namespace noKeyCloud.Contracts.Authenticate;

public record LoginInitResponse(
    string Salt,
    string B,
    Guid SessionId
    );