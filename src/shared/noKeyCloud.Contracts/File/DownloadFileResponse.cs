namespace noKeyCloud.Contracts.File;

public record DownloadFileResponse(
    Guid fileId,
    byte[] fileContent,
    string MimeType,
    byte[] EncryptedName,
    byte[] EncryptedKey,
    byte[] Checksum
    );
