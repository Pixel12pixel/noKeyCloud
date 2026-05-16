using Moq;
using noKeyCloud.Application.Abstractions.Services;
using noKeyCloud.Application.Features.Users.Logout;

namespace noKeyCloud.Application.UnitTests.Features.Users.Logout;

public class LogoutUserCommandHandlerTests
{
    private readonly Mock<IRefreshTokenProvider> _refreshTokenProviderMock;
    private readonly LogoutUserCommandHandler _handler;

    public LogoutUserCommandHandlerTests()
    {
        _refreshTokenProviderMock = new Mock<IRefreshTokenProvider>();
        _handler = new LogoutUserCommandHandler(_refreshTokenProviderMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRefreshToken_ReturnsSuccessMessageAndRemovesToken()
    {
        var userId = Guid.NewGuid();
        const string token = "valid-refresh-token";
        _refreshTokenProviderMock
            .Setup(x => x.GetRefreshTokenAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        var command = new LogoutUserCommand(userId, token);
        
        var result = await _handler.Handle(command, CancellationToken.None);
        
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("User logged out successfully.", result.Value.Message);
        _refreshTokenProviderMock.Verify(x => x.InvalidateRefreshTokenAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidOrRemovedToken_ReturnsFailure()
    {
        var userId = Guid.NewGuid();
        const string commandToken = "some-token";
        
        _refreshTokenProviderMock
            .Setup(x => x.GetRefreshTokenAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        var command = new LogoutUserCommand(userId, commandToken);
        
        var result = await _handler.Handle(command, CancellationToken.None);
        
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Contains("Token.NotFound", result.Error);
    }
    
    [Fact]
    public async Task Handle_MismatchedToken_ReturnsFailure()
    {
        var userId = Guid.NewGuid();
        const string commandToken = "some-token-from-client";
        const string cacheToken = "totally-different-token-in-cache";
        
        _refreshTokenProviderMock
            .Setup(x => x.GetRefreshTokenAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cacheToken);

        var command = new LogoutUserCommand(userId, commandToken);
        
        var result = await _handler.Handle(command, CancellationToken.None);
        
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Contains("Token.NotFound", result.Error);
        
        _refreshTokenProviderMock.Verify(x => x.InvalidateRefreshTokenAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}