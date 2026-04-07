using System.ComponentModel.DataAnnotations;

namespace noKeyCloud_api.Data.Models
{
    public class RecoveryMethods
    {
        [Key]
        public required Guid Id { get; set; }
        public required MethodType MethodType { get; set; }
        public required string RecoveryDataEncrypted { get; set; }
        public required bool IsActive { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required Users User { get; set; }
        public Guid UserId { get; set; }


    }
    public enum MethodType
    {
        Secret_recovery,
        File_recovery,
    }
}
