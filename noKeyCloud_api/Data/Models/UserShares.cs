using System.ComponentModel.DataAnnotations;

namespace noKeyCloud_api.Data.Models
{
    public class UserShares
    {
        [Key]
        public required Guid Id { get; set; }
        public required string ResourceType { get; set; }
        public required int ResourceId { get; set; }
        public required int PermissionLevel { get; set; }
        public required string SharedKeyEncrypted { get; set; }
        public required DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime RevokedAt { get; set; }
        public required Guid OwnerUserId { get; set; }
        public required Guid RecipientUserId { get; set; }
        public required Users User { get; set; }

    }
}
