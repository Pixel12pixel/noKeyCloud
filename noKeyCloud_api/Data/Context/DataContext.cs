using Microsoft.EntityFrameworkCore;
using noKeyCloud_api.Data.Models;

namespace noKeyCloud_api.Data.Context
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        public DbSet<AuthCredentials> AuthCredentials { get; set; }
        public DbSet<Files> Files { get; set; }
        public DbSet<Folders> Folders { get; set; }
        public DbSet<PublicLinks> PublicLinks { get; set; }
        public DbSet<RecoveryMethods> RecoveryMethods { get; set; }
        public DbSet<SecurityEvents> SecurityEvents { get; set; }
        public DbSet<Sessions> Sessions { get; set; }
        public DbSet<UserKeys> UserKeys { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<UserShares> UserShares { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>()
                .HasOne(u => u.Credentials)
                .WithOne(p => p.User)
                .HasForeignKey<AuthCredentials>(c => c.UserId);


            modelBuilder.Entity<UserShares>()
                .HasOne(u => u.User)
                .WithMany(s => s.Shares)
                .HasForeignKey(s => s.RecipientUserId);
            
            modelBuilder.Entity<UserShares>()
                .HasOne(u => u.User)
                .WithMany(s => s.Shares)
                .HasForeignKey(s => s.OwnerUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PublicLinks>()
                .HasOne(u => u.user)
                .WithMany(s => s.PublicLinks)
                .HasForeignKey(s => s.UserId);

            modelBuilder.Entity<Sessions>()
                .HasOne(u => u.User)
                .WithMany(s => s.Sessions)
                .HasForeignKey(s => s.UserId);

            modelBuilder.Entity<UserKeys>()
                .HasOne(u => u.User)
                .WithMany(s => s.UserKeys)
                .HasForeignKey(s => s.UserId);

            modelBuilder.Entity<RecoveryMethods>()
                .HasOne(u => u.User)
                .WithMany(s => s.RecoveryMethods)
                .HasForeignKey(s => s.UserId);

            modelBuilder.Entity<SecurityEvents>()
                .HasOne(u => u.User)
                .WithMany(s => s.SecurityEvents)
                .HasForeignKey(s => s.UserId);

            modelBuilder.Entity<Folders>()
                .HasOne(u => u.User)
                .WithMany(s => s.Folders)
                .HasForeignKey(s => s.UserId);

            modelBuilder.Entity<Files>()
                .HasOne(u => u.Folder)
                .WithMany(s => s.Files)
                .HasForeignKey(s => s.FolderId);

            modelBuilder.Entity<Folders>()
                .HasOne(u => u.ParentFolder)
                .WithMany(s => s.SubFolders)
                .HasForeignKey(s => s.ParentFolderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Files>()
                .HasOne(u => u.OwnerUser)
                .WithMany(s => s.Files)
                .HasForeignKey(s => s.OwnerUserId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }

}
