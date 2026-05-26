using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using noKeyCloud.Domain.Entities;

namespace noKeyCloud.Infrastructure.Configuration
{
    internal class FolderConfiguration : IEntityTypeConfiguration<Folder>
    {
        public void Configure(EntityTypeBuilder<Folder> builder)
        {
            builder.ToTable("Folders");
            builder.HasKey(p => p.Id);

            builder
                .HasOne(u => u.ParentFolder)
                .WithMany(s => s.SubFolders)
                .HasForeignKey(s => s.ParentFolderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(u => u.User)
                .WithMany(s => s.Folders)
                .HasForeignKey(s => s.UserId);
            
            builder
                .HasIndex(f => new { f.UserId, f.ParentFolderId })
                .IsUnique()
                .HasFilter("\"ParentFolderId\" IS NULL");
        }
    }
}
