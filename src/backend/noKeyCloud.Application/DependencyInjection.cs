using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace noKeyCloud.Application;

public static class DependencyInjection
{
    /// <summary>
    /// Configures application layer services, including MediatR for CQRS and other application-specific services(Validators, AutoMapper, etc.).
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        return services;
    }
}