using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using noKeyCloud.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace noKeyCloud.Infrastructure.Configuration
{
    internal class UserShareConfiguration : IEntityTypeConfiguration<UserShare>
    {
        public void Configure(EntityTypeBuilder<UserShare> builder)
        {
            builder.ToTable("UserShares");
            builder.HasKey(p => p.Id);

            builder
                .HasOne(u => u.Owner)
                .WithMany(s => s.UserShares)
                .HasForeignKey(s => s.OwnerUserId);

            builder
                .HasOne(u => u.Recipient)
                .WithMany()
                .HasForeignKey(s => s.RecipientUserId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
