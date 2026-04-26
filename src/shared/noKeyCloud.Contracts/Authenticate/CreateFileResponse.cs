namespace noKeyCloud.Contracts.Authenticate;

public record CreateFileResponse
(
    string FileName,
    Guid FileId,
    long FileSize
    );