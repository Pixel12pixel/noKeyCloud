using System.Text;
using Microsoft.EntityFrameworkCore;
using noKeyCloud.Infrastructure;
using Npgsql;
using Scalar.AspNetCore;
using noKeyCloud.Application.Abstractions.Services;
using noKeyCloud.Infrastructure.Services;
using noKeyCloud.Infrastructure.Repositories;
using noKeyCloud.Application.Abstractions.Repositories;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using noKeyCloud.Application.Features.Users.Register;

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

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddInfrastructure();

        builder.Services.AddAuthorization();

        var jwtSettings = builder.Configuration.GetSection("JwtSettings");
        var secretKey = Environment.GetEnvironmentVariable("JwtSettings__SecretKey");

        if (string.IsNullOrEmpty(secretKey))
        {
            throw new ArgumentNullException(nameof(secretKey), "JWT secret key must be provided in environment variables.");
        }

        builder.Services.AddAuthentication(options =>
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

        builder.Services.AddMediatR(cfg => {
            cfg.LicenseKey = Environment.GetEnvironmentVariable("KEY_LICENSE");
            cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly);
        });

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

        app.Run();
    }
    private static WebApplicationBuilder AddDbContext(WebApplicationBuilder builder)
    {
        var postgreUrl = Environment.GetEnvironmentVariable("DB_HOST");
        if (string.IsNullOrWhiteSpace(postgreUrl))
        {
            throw new InvalidOperationException("Required environment variable 'DB_HOST' is missing or empty. Configure 'DB_HOST' with the PostgreSQL connection URL before starting the application.");
        }

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