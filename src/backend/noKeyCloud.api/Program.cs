using noKeyCloud.api.CustomMiddleware;
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


        builder.Services.AddHsts(options =>
        {
            options.Preload = true;
            options.MaxAge = TimeSpan.FromDays(365);
        });

        builder.Services.AddHttpsRedirection(options =>
        {
            options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
            options.HttpsPort = 443;
        });

        builder.Services.AddControllers();

        builder.Services.AddOpenApi();

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddAuthorization();

        builder.Services.AddPresentation(configuration: builder.Configuration);
        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(config: builder.Configuration);



        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            dbContext.Database.EnsureCreated();
        }

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }
        else
        {
            app.UseHsts();
        }

        app.Use(async (context, next) =>
        {
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Append("X-Xss-Protection", "1; mode=block");
            context.Response.Headers.Append("X-Frame-Options", "DENY");
            context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

            await next(context);
        });

        app.UseContentSecurityPolicy();
        app.UseHttpsRedirection();

        app.UseCors("FrontendOrigin");

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();


        app.Run();
    }
}