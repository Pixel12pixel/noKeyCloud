using System.ComponentModel.DataAnnotations;

namespace noKeyCloud_api.Data.Models
{
    public class Files
    {
        [Key]
        public required Guid Id { get; set; }
        public required string FileNameEncrypted { get; set; }
        public required string MimeType { get; set; }
        public required int SizeBytes { get; set; }
        public required string StoragePath { get; set; }
        public required string FileKeyEncrypted { get; set; }
        public required string CheckSum { get; set; }
        public required DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public required Folders Folder { get; set; }
        public Guid FolderId { get; set; }
        public Guid OwnerUserId { get; set; }
        public required Users OwnerUser { get; set; }

    }
}
