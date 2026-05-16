using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Application.Abstractions.Services;
using noKeyCloud.Infrastructure.Repositories;
using noKeyCloud.Infrastructure.Services;
using Npgsql;

namespace noKeyCloud.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Configures infrastructure layer services, including database context, repositories, and other infrastructure-specific services. Reads PostgreSQL connection details from the 'DB_HOST' environment variable and configures the DbContext accordingly.
    /// </summary>
    /// <param name="config">Builder configuration instance</param>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        
        // Register infrastructure services and repositories
        
        services.AddSingleton<ISrpSessionStore, InMemorySrpSessionStore>();

        services.AddScoped<IFolderRepository, FolderRepository>();
        
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IJwtService, JwtService>();
        
        services.AddScoped<IFileRepository, FileRepository>();
        
        
        
        
        
        // PostgreSQL and DbContext configuration from environment variables
        
        var postgreUrl = Environment.GetEnvironmentVariable("DB_HOST");
        if (string.IsNullOrWhiteSpace(postgreUrl))
        {
            throw new InvalidOperationException("Required environment variable 'DB_HOST' is missing or empty. Configure 'DB_HOST' with the PostgreSQL connection URL before starting the application.");
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

        services.AddDbContext<DataContext>(opts =>
            opts.UseNpgsql(
                npgsqlBuilder.ConnectionString,
                sqlOpts => sqlOpts.EnableRetryOnFailure()
            )
        );
        
        return services;
    }
}