namespace noKeyCloud.api.Controllers.DTOs;

public record CreateFolderRequest(
    byte[] EncryptedName,
    byte[] EncryptedKey,
    Guid? ParentFolderId);