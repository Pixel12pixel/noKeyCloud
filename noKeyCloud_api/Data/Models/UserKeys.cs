namespace noKeyCloud_api.Data.Models
{
    public class UserKeys
    {
        public required Guid Id { get; set; }
        public required string PublicKey { get; set; }
        public required string EncryptedPrivateKey { get; set; }
        public required string KeyVersion { get; set; }
        public required bool IsActive { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required Users User { get; set; }
        public Guid UserId { get; set; }

    }
}
