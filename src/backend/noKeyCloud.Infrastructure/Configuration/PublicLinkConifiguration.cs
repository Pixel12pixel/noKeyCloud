using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using noKeyCloud.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace noKeyCloud.Infrastructure.Configuration
{
    internal class PublicLinkConifiguration : IEntityTypeConfiguration<PublicLink>
    {
        public void Configure(EntityTypeBuilder<PublicLink> builder)
        {
            builder.ToTable("PublicLinks");
            builder.HasKey(p => p.Id);

            builder
                .HasOne(u => u.User)
                .WithMany(s => s.PublicLinks)
                .HasForeignKey(s => s.UserId);
        }
    }
}
