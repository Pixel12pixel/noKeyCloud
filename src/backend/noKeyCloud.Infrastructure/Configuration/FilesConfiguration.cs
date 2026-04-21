using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace noKeyCloud.Infrastructure.Configuration
{
    internal class FilesConfiguration : IEntityTypeConfiguration<Domain.Entities.File>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.File> builder)
        {
            builder.ToTable("Files");
            builder.HasKey(p => p.Id);

            builder
                .HasOne(u => u.ParentFolder)
                .WithMany(s => s.Files)
                .HasForeignKey(s => s.ParentFolderId);

            builder
                .HasOne(u => u.OwnerUser)
                .WithMany(s => s.Files)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
