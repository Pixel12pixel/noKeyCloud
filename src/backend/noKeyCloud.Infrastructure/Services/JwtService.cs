using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using noKeyCloud.Application.Abstractions.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace noKeyCloud.Infrastructure.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> JwtTokenService(Guid? Id)
        {
            var JwtSettings = _configuration.GetSection("JwtSettings");
            var secretKeyString = JwtSettings["SecretKey"];

            if (string.IsNullOrEmpty(secretKeyString))
            {
                throw new Exception("Missing JWT SecretKey environment variable");
            }

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKeyString));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var validIssuers = JwtSettings.GetSection("ValidIssuer").Get<string[]>() ?? Array.Empty<string>();
            var validAudiences = JwtSettings.GetSection("ValidAudience").Get<string[]>() ?? Array.Empty<string>();

            var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: validIssuers.FirstOrDefault(),
                audience: validAudiences.FirstOrDefault(),
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(JwtSettings["Lifetime"] ?? "60")),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
