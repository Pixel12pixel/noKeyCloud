using Microsoft.EntityFrameworkCore;
using noKeyCloud.Infrastructure;
using Npgsql;
using Scalar.AspNetCore;

namespace noKeyCloud.api;

public class Program
{
    public static void Main(string[] args)
    {
        DotNetEnv.Env.Load();

        var builder = WebApplication.CreateBuilder(args);

        builder = AddDbContext(builder);
        

        builder.Services.AddControllers();
        
        builder.Services.AddOpenApi();

        var app = builder.Build();

        
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
    private static WebApplicationBuilder AddDbContext(WebApplicationBuilder builder)
    {
        var envFilePath = ".env";
        var postgreUrl = Environment.GetEnvironmentVariable("DB_HOST")!;

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

        builder.Services.AddDbContext<DataContext>(opts =>
            opts.UseNpgsql(
                npgsqlBuilder.ConnectionString,
                sqlOpts => sqlOpts.EnableRetryOnFailure()
            )
        );
        return builder;
    }
}