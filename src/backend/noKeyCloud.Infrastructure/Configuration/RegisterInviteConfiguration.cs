using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using noKeyCloud.Domain.Entities;

namespace noKeyCloud.Infrastructure.Configuration;

internal class RegisterInviteConfiguration : IEntityTypeConfiguration<RegisterInvite>
{
    public void Configure(EntityTypeBuilder<RegisterInvite> builder)
    {
        builder.ToTable("RegisterInvites");
        
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Code)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(e => e.IsUsed)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.ExpiresAt)
            .IsRequired(false);
        
        builder.HasIndex(e => e.Code)
            .IsUnique();
    }
}