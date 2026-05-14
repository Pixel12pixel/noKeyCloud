namespace noKeyCloud.Contracts.File;

public record CreateFileRequest
(
    string UserId,
    string FileName,
    string FolderId
    );