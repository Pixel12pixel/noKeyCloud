using Microsoft.Extensions.DependencyInjection;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Application.Abstractions.Services;
using noKeyCloud.Infrastructure.Repositories;
using noKeyCloud.Infrastructure.Services;

namespace noKeyCloud.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<ISrpSessionStore, InMemorySrpSessionStore>();

        services.AddScoped<IFolderRepository, FolderRepository>();
        
        services.AddScoped<IUserRepository, UserRepository>();
        
        return services;
    }
}