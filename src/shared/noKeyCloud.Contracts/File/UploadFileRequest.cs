namespace noKeyCloud.Contracts.File;

public record UploadFileRequest
(
    byte[] FileName,
    string MimeType,
    long SizeBytes,
    byte[] EncryptedKey,
    byte[] Checksum,
    string FolderId,
    byte[] FileContent
);