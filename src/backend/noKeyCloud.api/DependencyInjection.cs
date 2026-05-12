using MediatR;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Application.Abstractions.Services;
using noKeyCloud.Infrastructure.Repositories;
using noKeyCloud.Infrastructure.Services;

namespace noKeyCloud.api;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<ISrpSessionStore, InMemorySrpSessionStore>();

        services.AddScoped<IFolderRepository, FolderRepository>();
        
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IJwtService, JwtService>();

        services.AddScoped<IMediator, Mediator>();

        return services;
    }
}