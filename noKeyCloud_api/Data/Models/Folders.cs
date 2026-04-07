using System.ComponentModel.DataAnnotations;

namespace noKeyCloud_api.Data.Models
{
    public class Folders
    {
        [Key]
        public required Guid Id { get; set; }
        public required string NameEncrypted { get; set; }
        public required string FolderKeyEncrypted { get; set; }
        public required DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public required Users User { get; set; }
        public Guid UserId { get; set; }
        public ICollection<Files> Files { get; set; }
        public ICollection<Folders> SubFolders { get; set; }
        public required Folders? ParentFolder { get; set; }
        public required Guid? ParentFolderId { get; set; }
    }
}
