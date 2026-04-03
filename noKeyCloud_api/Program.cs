using Microsoft.EntityFrameworkCore;
using noKeyCloud_api.Data.Context;
using Npgsql;

namespace noKeyCloud_api;

public class Program
{
    public static void Main(string[] args)
    {
        DotNetEnv.Env.Load();

        var builder = WebApplication.CreateBuilder(args);

        builder = AddDbContext(builder);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
    private static WebApplicationBuilder AddDbContext(WebApplicationBuilder builder)
    {
        string envFilePath = ".env";
        string postgreUrl;
        postgreUrl = Environment.GetEnvironmentVariable("DB_HOST");

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