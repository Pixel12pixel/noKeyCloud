namespace noKeyCloud.Contracts.File;

public record CreateFileResponse
(
    string FileName,
    Guid FileId,
    long FileSize
    );