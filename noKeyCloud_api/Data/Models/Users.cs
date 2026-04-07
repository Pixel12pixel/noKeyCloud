using System.ComponentModel.DataAnnotations;
using System.Data;

namespace noKeyCloud_api.Data.Models
{
    public class Users
    {
        [Key]
        public required Guid Id { get; set; }
        public required string Email { get; set; }
        public required string? Username { get; set; }
        public required bool? IsActive { get; set; }
        public required DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public AuthCredentials Credentials { get; set; }
        public ICollection<UserShares> Shares{ get; set; }
        public ICollection<PublicLinks> PublicLinks{ get; set; }
        public ICollection<Sessions> Sessions{ get; set; }
        public ICollection<UserKeys> UserKeys{ get; set; }
        public ICollection<RecoveryMethods> RecoveryMethods{ get; set; }
        public ICollection<SecurityEvents> SecurityEvents{ get; set; }
        public ICollection<Folders> Folders{ get; set; }
        public ICollection<Files> Files{ get; set; }
    }
}
