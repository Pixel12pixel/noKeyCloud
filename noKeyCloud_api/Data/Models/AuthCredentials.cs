using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace noKeyCloud_api.Data.Models
{
    public class AuthCredentials
    {
        [Key]
        public required Guid Id { get; set; }
        public required string Salt { get; set; }
        public required string Verifier { get; set; }
        public required DateTime CreatedAt { get; set; }
        public DateTime CreationDate { get; set; }
        public Guid UserId { get; set; }
        public Users User { get; set; }
    }

}
