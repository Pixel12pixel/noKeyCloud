using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using noKeyCloud.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace noKeyCloud.Infrastructure.Configuration
{
    internal class RecoveryMethodConfiguration : IEntityTypeConfiguration<RecoveryMethod>
    {
        public void Configure(EntityTypeBuilder<RecoveryMethod> builder)
        {
            builder.ToTable("RecoveryMethods");
            builder.HasKey(p => p.Id);

            builder
                .HasOne(u => u.User)
                .WithMany(s => s.RecoveryMethods)
                .HasForeignKey(s => s.UserId);
        }
    }
}
