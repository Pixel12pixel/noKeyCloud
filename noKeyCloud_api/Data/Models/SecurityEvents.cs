using System.ComponentModel.DataAnnotations;

namespace noKeyCloud_api.Data.Models
{
    public class SecurityEvents
    {
        [Key]
        public required Guid Id { get; set; }
        public required EventType EventType { get; set; }
        public required string Details { get; set; }
        public required string IpAddress { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required Users User { get; set; }
        public Guid UserId { get; set; }

    }
    public enum EventType
    {
        Login_attempt,
        Password_change,
        Recovery_method_added,
        Recovery_method_removed,
        Account_lockout,
    }
}
