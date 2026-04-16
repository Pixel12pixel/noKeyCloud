using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using noKeyCloud.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace noKeyCloud.Infrastructure.Configuration
{
    internal class UserKeyConfiguration : IEntityTypeConfiguration<UserKey>
    {
        public void Configure(EntityTypeBuilder<UserKey> builder)
        {
            builder.ToTable("UserKeys");
            builder.HasKey(p => p.Id);

            builder
                .HasOne(u => u.User)
                .WithMany(s => s.UserKeys)
                .HasForeignKey(s => s.UserId);

        }
    }
}
