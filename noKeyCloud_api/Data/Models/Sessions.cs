using System.ComponentModel.DataAnnotations;
using System.Net;

namespace noKeyCloud_api.Data.Models
{
    public class Sessions
    {
        [Key]
        public required Guid Id { get; set; }
        public required string TokenHash { get; set; }
        public required string DeviceNametag { get; set; }
        public required string IpAddress { get; set; }
        public required string UserAgent { get; set; }
        public required DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime RevokedAt { get; set; }
        public required Users User { get; set; }
        public Guid UserId { get; set; }


    }
}
