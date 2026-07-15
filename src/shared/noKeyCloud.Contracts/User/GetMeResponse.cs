namespace noKeyCloud.Contracts.User;

public record GetMeResponse(
    string UserId,
    string Username,
    string Email,
    string RootFolderId,
    byte[] RootFolderKey,
    bool IsAdmin
    );