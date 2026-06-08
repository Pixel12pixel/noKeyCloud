using Moq;
using noKeyCloud.Application.Abstractions.Services;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Application.Features.Users.RefreshSession;
using noKeyCloud.Domain.Entities;

namespace noKeyCloud.Application.UnitTests.Features.Users.RefreshSession;

public class RefreshSessionHandlerTests
{
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly Mock<IRefreshTokenProvider> _refreshTokenProviderMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly RefreshSessionHandler _handler;

    public RefreshSessionHandlerTests()
    {
        _jwtServiceMock = new Mock<IJwtService>();
        _refreshTokenProviderMock = new Mock<IRefreshTokenProvider>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _handler = new RefreshSessionHandler(_jwtServiceMock.Object, _refreshTokenProviderMock.Object, _userRepositoryMock.Object);
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
            
        _userRepositoryMock.Setup(x => x.GetUserByUserId(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User(userId, "test@test.com", "test", new byte[0], new byte[0], false));

        _jwtServiceMock.Setup(x => x.JwtTokenService(userId, false))
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