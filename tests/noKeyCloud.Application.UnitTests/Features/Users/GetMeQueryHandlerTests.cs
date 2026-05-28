using Moq;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Application.Features.Users.GetMe;
using noKeyCloud.Domain.Entities;

namespace noKeyCloud.Application.UnitTests.Features.Users;

public class GetMeQueryHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly GetMeQueryHandler _handler;

    public GetMeQueryHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _handler = new GetMeQueryHandler(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_UserExists_ReturnsSuccessAndUserResponse()
    {
        var userId = Guid.NewGuid();
        var query = new GetMeQuery(userId);
        
        var user = new User(userId, "test@email.com", "testUser", new byte[] { 1 }, new byte[] { 2 });
        
        _userRepositoryMock
            .Setup(x => x.GetUserByUserId(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        
        var result = await _handler.Handle(query, CancellationToken.None);
        
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(userId.ToString(), result.Value.UserId);
        Assert.Equal("testUser", result.Value.Username);
        Assert.Equal("test@email.com", result.Value.Email);
    }

    [Fact]
    public async Task Handle_UserDoesNotExist_ReturnsFailure()
    {
        var userId = Guid.NewGuid();
        var query = new GetMeQuery(userId);

        _userRepositoryMock
            .Setup(x => x.GetUserByUserId(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        
        var result = await _handler.Handle(query, CancellationToken.None);
        
        Assert.False(result.IsSuccess);
        Assert.Equal("User not found.", result.Error);
        Assert.Null(result.Value);
    }
}