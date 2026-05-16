using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Moq;
using noKeyCloud.Infrastructure.Services;
using Xunit;

namespace noKeyCloud.Application.UnitTests.Features.Services
{
    public class JwtServiceTests
    {
        private readonly Mock<IConfigurationSection> _jwtSettingsSectionMock;

        public JwtServiceTests()
        {
            _jwtSettingsSectionMock = new Mock<IConfigurationSection>();
        }

        [Fact]
        public async Task JwtTokenService_ThrowsInvalidOperationException_WhenSecretKeyMissing()
        {

            Environment.SetEnvironmentVariable("JwtSettings__SecretKey", null);
            
            IConfiguration configuration = new ConfigurationBuilder().AddEnvironmentVariables().Build();

            var jwtService = new JwtService(configuration);
            var id = Guid.NewGuid();

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => jwtService.JwtTokenService(id));
            Assert.Equal("Missing JWT SecretKey environment variable", exception.Message);
        }

        [Fact]
        public async Task JwtTokenService_ReturnsValidToken_WhenConfiguredCorrectly()
        {
            var secretKey = "A_VERY_SECRET_KEY_THAT_IS_LONG_ENOUGH_FOR_HMACSHA256";
            Environment.SetEnvironmentVariable("JwtSettings__SecretKey", secretKey);

            IConfiguration configuration = new ConfigurationBuilder().AddEnvironmentVariables().Build();

            _jwtSettingsSectionMock.Setup(s => s["Lifetime"]).Returns("60");

            var validIssuerSectionMock = new Mock<IConfigurationSection>();
            validIssuerSectionMock.Setup(s => s.Value).Returns("");
            _jwtSettingsSectionMock.Setup(s => s.GetSection("ValidIssuer")).Returns(validIssuerSectionMock.Object);

            var validAudienceSectionMock = new Mock<IConfigurationSection>();
            validAudienceSectionMock.Setup(s => s.Value).Returns("");
            _jwtSettingsSectionMock.Setup(s => s.GetSection("ValidAudience")).Returns(validAudienceSectionMock.Object);

            var jwtService = new JwtService(configuration);
            var id = Guid.NewGuid();


            var tokenString = await jwtService.JwtTokenService(id);

            Assert.False(string.IsNullOrEmpty(tokenString));

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(tokenString);

            Assert.Equal(id.ToString(), token.Subject);
            Assert.Contains(token.Claims, c => c.Type == JwtRegisteredClaimNames.Jti);
        }
    }
}
