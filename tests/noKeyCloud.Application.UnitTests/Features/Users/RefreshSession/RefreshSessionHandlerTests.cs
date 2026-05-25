using Moq;
using noKeyCloud.Application.Abstractions.Services;
using noKeyCloud.Application.Features.Users.RefreshSession;

namespace noKeyCloud.Application.UnitTests.Features.Users.RefreshSession;

public class RefreshSessionHandlerTests
{
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly Mock<IRefreshTokenProvider> _refreshTokenProviderMock;
    private readonly RefreshSessionHandler _handler;

    public RefreshSessionHandlerTests()
    {
        _jwtServiceMock = new Mock<IJwtService>();
        _refreshTokenProviderMock = new Mock<IRefreshTokenProvider>();
        _handler = new RefreshSessionHandler(_jwtServiceMock.Object, _refreshTokenProviderMock.Object);
    }

    [Fact]
    public async Task Handle_ValidToken_ReturnsNewTokenAndJwt()
    {
        var userId = Guid.NewGuid();
        var command = new RefreshSessionCommand(userId, "old-valid-token");
        
        _refreshTokenProviderMock.Setup(x => x.GetRefreshTokenAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync("old-valid-token");
        
        _refreshTokenProviderMock.Setup(x => x.GenerateRefreshToken())
            .Returns("new-refresh-token");
            
        _jwtServiceMock.Setup(x => x.JwtTokenService(userId))
            .ReturnsAsync("new-jwt-token");

        var result = await _handler.Handle(command, CancellationToken.None);
        
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        
        Assert.Equal("new-refresh-token", result.Value!.RefreshToken);
        Assert.Equal("new-jwt-token", result.Value!.JwtToken);
        
        _refreshTokenProviderMock.Verify(x => x.StoreRefreshTokenAsync(
            userId, 
            "new-refresh-token", 
            TimeSpan.FromHours(24), 
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidOrExpiredToken_ReturnsFailure()
    {
        var userId = Guid.NewGuid();
        var command = new RefreshSessionCommand(userId, "invalid-token");
        
        _refreshTokenProviderMock.Setup(x => x.GetRefreshTokenAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        var result = await _handler.Handle(command, CancellationToken.None);
        
        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid or expired refresh token.", result.Error);
    }
}