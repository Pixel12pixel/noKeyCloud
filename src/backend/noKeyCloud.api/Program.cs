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



        // Create the WebApplication builder
        var builder = WebApplication.CreateBuilder();

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

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();


        // Run the application
        app.Run();
    }
}