using System.ComponentModel.DataAnnotations;

namespace noKeyCloud_api.Data.Models
{
    public class PublicLinks
    {
        [Key]
        public required Guid Id { get; set; }
        public required string ResourceType { get; set; }
        public required int ResourceId { get; set; }
        public required string Publictoken { get; set; }
        public required string PasswordHash { get; set; }
        public required string LinkKeyEncrypted { get; set; }
        public required DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime RevokedAt { get; set; }
        public required Users user { get; set; }
        public Guid UserId { get; set; }
    }
}
