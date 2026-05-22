using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Npgsql;

namespace noKeyCloud.Infrastructure;

public class DataContextFactory
    : IDesignTimeDbContextFactory<DataContext>
{
    public DataContext CreateDbContext(string[] args)
    {
        DotNetEnv.Env.Load();

        var postgreUrl = Environment.GetEnvironmentVariable("DB_HOST");

        if (string.IsNullOrWhiteSpace(postgreUrl))
        {
            throw new InvalidOperationException(
                "DB_HOST environment variable is missing.");
        }

        var uri = new Uri(postgreUrl);
        var userInfo = uri.UserInfo.Split(':');

        var npgsqlBuilder = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.Port,
            Database = uri.AbsolutePath.TrimStart('/'),
            Username = userInfo[0],
            Password = userInfo[1],
            Pooling = true,
        };

        var optionsBuilder =
            new DbContextOptionsBuilder<DataContext>();

        optionsBuilder.UseNpgsql(
            npgsqlBuilder.ConnectionString);

        return new DataContext(optionsBuilder.Options);
    }
}