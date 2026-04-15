using Microsoft.EntityFrameworkCore;
using noKeyCloud.Domain.Entities;
using File = noKeyCloud.Domain.Entities.File;

namespace noKeyCloud.Infrastructure;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }
    public DbSet<File> Files { get; set; }
    public DbSet<Folder> Folders { get; set; }
    public DbSet<PublicLink> PublicLinks { get; set; }
    public DbSet<RecoveryMethod> RecoveryMethods { get; set; }
    public DbSet<SecurityEvent> SecurityEvents { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<UserKey> UserKeys { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserShare> UserShares { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(DataContext).Assembly);
    }
}