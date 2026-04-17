namespace noKeyCloud.Contracts.Authenticate;

public record LoginInitRequest(
    string Username,
    string Email,
    string A
    );