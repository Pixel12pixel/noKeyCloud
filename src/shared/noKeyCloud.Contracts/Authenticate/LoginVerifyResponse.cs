namespace noKeyCloud.Contracts.Authenticate;

public record LoginVerifyResponse
(
    byte[] M2,
    string RootFolderId,
    byte[] EncryptedMasterKey,
    byte[] KeySalt,
    byte[] RootFolderKey
);
