using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using noKeyCloud.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace noKeyCloud.Infrastructure.Configuration
{
    internal class SecurityEventConfiguration : IEntityTypeConfiguration<SecurityEvent>
    {
        public void Configure(EntityTypeBuilder<SecurityEvent> builder)
        {
            builder.ToTable("SecurityEvents");
            builder.HasKey(p => p.Id);

            builder
                 .HasOne(u => u.User)
                 .WithMany(s => s.SecurityEvents)
                 .HasForeignKey(s => s.UserId);

        }
    }
}
