namespace noKeyCloud.Contracts.File;

public record CreateFileRequest
(
    string FileName,
    string FolderId
    );