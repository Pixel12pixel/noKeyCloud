using Moq;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Application.Abstractions.Services;
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
        var command = new RefreshSessionCommand("old-valid-token");

        _refreshTokenProviderMock.Setup(x => x.GetUserIdByRefreshTokenAsync("old-valid-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(userId);

        _refreshTokenProviderMock.Setup(x => x.GetRefreshTokenAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync("old-valid-token");

        _refreshTokenProviderMock.Setup(x => x.GenerateRefreshToken())
            .Returns("new-refresh-token");

        _userRepositoryMock.Setup(x => x.GetUserByUserId(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User(userId, "test@test.com", "test", new byte[0], new byte[0], new byte[0], new byte[0], false));

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
    public async Task Handle_UserIdNotFound_ReturnsFailure()
    {

        var command = new RefreshSessionCommand("invalid-token");

        _refreshTokenProviderMock.Setup(x => x.GetUserIdByRefreshTokenAsync("invalid-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid or expired refresh token.", result.Error);
    }

    [Fact]
    public async Task Handle_StoredTokenMismatch_ReturnsFailure()
    {
        var userId = Guid.NewGuid();
        var command = new RefreshSessionCommand("provided-token");

        _refreshTokenProviderMock.Setup(x => x.GetUserIdByRefreshTokenAsync("provided-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(userId);

        _refreshTokenProviderMock.Setup(x => x.GetRefreshTokenAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync("different-stored-token");

        var result = await _handler.Handle(command, CancellationToken.None);


        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid or expired refresh token.", result.Error);
    }

    [Fact]
    public async Task Handle_GenerateRefreshTokenFails_ReturnsFailure()
    {
        var userId = Guid.NewGuid();
        var command = new RefreshSessionCommand("valid-token");

        _refreshTokenProviderMock.Setup(x => x.GetUserIdByRefreshTokenAsync("valid-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(userId);

        _refreshTokenProviderMock.Setup(x => x.GetRefreshTokenAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync("valid-token");

        _refreshTokenProviderMock.Setup(x => x.GenerateRefreshToken())
            .Returns(string.Empty);

        var result = await _handler.Handle(command, CancellationToken.None);


        Assert.False(result.IsSuccess);
        Assert.Equal("Failed to generate a new refresh token.", result.Error);
    }

    [Fact]
    public async Task Handle_GenerateJwtFails_ReturnsFailure()
    {
        var userId = Guid.NewGuid();
        var command = new RefreshSessionCommand("valid-token");

        _refreshTokenProviderMock.Setup(x => x.GetUserIdByRefreshTokenAsync("valid-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(userId);

        _refreshTokenProviderMock.Setup(x => x.GetRefreshTokenAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync("valid-token");

        _refreshTokenProviderMock.Setup(x => x.GenerateRefreshToken())
            .Returns("new-refresh-token");

        _userRepositoryMock.Setup(x => x.GetUserByUserId(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User(userId, "test@test.com", "test", new byte[0], new byte[0], new byte[0], new byte[0], false));

        _jwtServiceMock.Setup(x => x.JwtTokenService(userId, false))
            .ReturnsAsync(string.Empty);

        var result = await _handler.Handle(command, CancellationToken.None);


        Assert.False(result.IsSuccess);
        Assert.Equal("Failed to generate a new JWT token.", result.Error);
    }
    [Fact]
    public async Task Handle_ValidToken_ReturnsNewTokenAndJwt_AndRevokesOldToken()
    {
        var userId = Guid.NewGuid();
        var oldToken = "old-valid-token";
        var command = new RefreshSessionCommand(oldToken);

        _refreshTokenProviderMock.Setup(x => x.GetUserIdByRefreshTokenAsync(oldToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userId);

        _refreshTokenProviderMock.Setup(x => x.GetRefreshTokenAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(oldToken);

        _refreshTokenProviderMock.Setup(x => x.GenerateRefreshToken())
            .Returns("new-refresh-token");

        _userRepositoryMock.Setup(x => x.GetUserByUserId(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User(userId, "test@test.com", "test", new byte[0], new byte[0], new byte[0], new byte[0], false));

        _jwtServiceMock.Setup(x => x.JwtTokenService(userId, false))
            .ReturnsAsync("new-jwt-token");

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("new-refresh-token", result.Value!.RefreshToken);
        Assert.Equal("new-jwt-token", result.Value!.JwtToken);

        _refreshTokenProviderMock.Verify(x => x.RevokeRefreshTokenAsync(
            oldToken,
            It.IsAny<CancellationToken>()
        ), Times.Once);

        _refreshTokenProviderMock.Verify(x => x.StoreRefreshTokenAsync(
           userId,
           "new-refresh-token",
           TimeSpan.FromHours(24),
           It.IsAny<CancellationToken>()
       ), Times.Once);
    }
}