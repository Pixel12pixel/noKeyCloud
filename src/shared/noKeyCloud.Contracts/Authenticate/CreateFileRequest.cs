namespace noKeyCloud.Contracts.Authenticate;

public record CreateFileRequest
(
    string UserId,
    string FileName,
    string FolderId
    );