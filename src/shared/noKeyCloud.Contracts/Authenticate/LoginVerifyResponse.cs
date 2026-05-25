namespace noKeyCloud.Contracts.Authenticate;

public record LoginVerifyResponse
(
    string UserId,
    string M2,
    string RootFolderId,
    DateTime AccessTokenExpiresAt
);
