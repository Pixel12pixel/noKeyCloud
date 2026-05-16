using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace noKeyCloud.api;

public static class DependencyInjection
{
    /// <summary>
    /// Configures presentation layer services, including JWT authentication.
    /// </summary>
    /// <param name="configuration">Builder configuration instance</param>
    public static IServiceCollection AddPresentation(this IServiceCollection services,  IConfiguration configuration)
    {
        
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"];

        if (string.IsNullOrEmpty(secretKey))
        {
            throw new ArgumentNullException(nameof(secretKey), "JWT secret key must be provided in environment variables.");
        }

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuers = jwtSettings.GetSection("ValidIssuer").Get<string[]>(),
                    ValidAudiences = jwtSettings.GetSection("ValidAudience").Get<string[]>(),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };
            });
        
        return services;
    }
}