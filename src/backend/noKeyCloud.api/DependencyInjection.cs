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
        
        if (Encoding.UTF8.GetBytes(secretKey).Length < 16)
        {
            throw new InvalidOperationException("CRITICAL RUNTIME ERROR: The 'JwtSettings__SecretKey' provided in your .env configuration is too short. " +
                                                "Symmetric HS256 JWT keys must be strictly greater than 16 bytes (128-bits). " +
                                                $"Your current key is only {Encoding.UTF8.GetBytes(secretKey).Length} bytes long. Please generate a longer secure string.");
        }

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        if (context.Request.Cookies.ContainsKey("access_token"))
                        {
                            context.Token = context.Request.Cookies["access_token"];
                        }
                        return Task.CompletedTask;
                    }
                };
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