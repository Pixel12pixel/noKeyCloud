using noKeyCloud.Application;
using noKeyCloud.Infrastructure;
using Scalar.AspNetCore;

namespace noKeyCloud.api;

public class Program
{
    /// <summary>
    /// Main Program Entrypoint
    /// </summary>
    public static void Main()
    {

        // Load environment variables from .env file
        DotNetEnv.Env.Load();

        var frontendUrl = Environment.GetEnvironmentVariable("FRONTEND_URL");
        
        if (string.IsNullOrWhiteSpace(frontendUrl))
        {
            throw new InvalidOperationException("CRITICAL ERROR: 'FRONTEND_URL' environment variable is missing. It is required for CORS and security.");
        }

        if (!Uri.TryCreate(frontendUrl, UriKind.Absolute, out var parsedUri) || (parsedUri.Scheme != "http" && parsedUri.Scheme != "https"))
        {
            throw new InvalidOperationException($"CRITICAL ERROR: 'FRONTEND_URL' ({frontendUrl}) is malformed. It must be a valid absolute HTTP or HTTPS URL (e.g., http://localhost:5173)");
        }

        // Create the WebApplication builder
        var builder = WebApplication.CreateBuilder();
        
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("FrontendOrigin", policy =>
            {
                policy.WithOrigins(frontendUrl.TrimEnd('/')) 
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        builder.Services.AddControllers();

        builder.Services.AddOpenApi();

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddAuthorization();

        builder.Services.AddPresentation(configuration: builder.Configuration);
        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(config: builder.Configuration);



        // Build the application
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.UseHttpsRedirection();
        
        app.UseCors("FrontendOrigin");

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();


        // Run the application
        app.Run();
    }
}