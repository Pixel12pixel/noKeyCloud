using Moq;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Application.Abstractions.Services;
using noKeyCloud.Application.Features.Users.RemoveUser;
using noKeyCloud.Contracts.Common;
using noKeyCloud.Domain.Entities;

namespace noKeyCloud.Application.UnitTests.Features.Users.RemoveUser;

public class RemoveUserTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IRefreshTokenProvider> _refreshTokenProvider;
    private readonly RemoveUserCommandHandler _handler;
    
    public RemoveUserTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _refreshTokenProvider = new Mock<IRefreshTokenProvider>();
        
        _handler = new RemoveUserCommandHandler(_userRepositoryMock.Object,  _refreshTokenProvider.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserDoesNotExist()
    {
        var command = new RemoveUserCommand(Guid.NewGuid());

        _userRepositoryMock
            .Setup(x => x.GetUserByUserId(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        
        var result = await _handler.Handle(
            command,
            CancellationToken.None);
        
        Assert.False(result.IsSuccess);
        Assert.Equal("User not found", result.Error);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenUserIsRemoved()
    {
        var fakeSalt = new byte[] { 1, 2, 3 };
        var fakeVerifier = new byte[256]; 
        fakeVerifier[0] = 1;
        var userId = Guid.NewGuid();
        
        var fakeUser = new User(
            userId, 
            "test@email.com", 
            "testuser", 
            fakeSalt, 
            fakeVerifier
        );

        var command = new RemoveUserCommand(userId);

        _userRepositoryMock
            .Setup(x => x.GetUserByUserId(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeUser);

        _userRepositoryMock
            .Setup(x => x.RemoveUserByUser(fakeUser, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        _refreshTokenProvider
            .Setup(x => x.InvalidateRefreshTokenAsync(userId, It.IsAny<CancellationToken>()));
        
        var result = await _handler.Handle(command, CancellationToken.None);
        
        Assert.True(result.IsSuccess);
        Assert.Equal("User successfully removed", result.Value.Message);
    }
}