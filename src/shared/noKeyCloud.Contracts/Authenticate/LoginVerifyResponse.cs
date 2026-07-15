namespace noKeyCloud.Contracts.Authenticate;

public record LoginVerifyResponse
(
    byte[] M2,
    byte[] EncryptedMasterKey,
    byte[] KeySalt
);
