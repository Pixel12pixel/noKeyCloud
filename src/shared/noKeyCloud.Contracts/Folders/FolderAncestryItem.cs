namespace noKeyCloud.Contracts.Folders;

public record FolderAncestryItem(
    Guid Id,
    byte[] EncryptedName,
    byte[] EncryptedKey
);