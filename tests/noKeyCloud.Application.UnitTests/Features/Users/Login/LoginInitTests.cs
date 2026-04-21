using Moq;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Application.Abstractions.Services;
using noKeyCloud.Application.Features.Users.LoginInit;
using noKeyCloud.Domain.Entities;
using Org.BouncyCastle.Crypto.Agreement.Srp;

namespace noKeyCloud_apiUnitTests.Features.Users.Login;

public class LoginInitTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ISrpSessionStore> _sessionStoreMock;
    private readonly LoginInitCommandHandler _handler;
    
    public LoginInitTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _sessionStoreMock = new Mock<ISrpSessionStore>();
        
        _handler = new LoginInitCommandHandler(_userRepositoryMock.Object, _sessionStoreMock.Object);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ShouldReturnFailureResult()
    {
        _userRepositoryMock
            .Setup(repo => repo.GetUserByUsernameOrEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var command = new LoginInitCommand("wrong_user", "wrong@email.com", "dummy_A_base64_string");
        
        var result = await _handler.Handle(command, CancellationToken.None);
        
        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid credentials.", result.Error);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task Handle_WhenUserExists_ShouldReturnSuccessWithSrpData()
    {
        var fakeSalt = new byte[] { 1, 2, 3 };
        var fakeVerifier = new byte[256]; 
        fakeVerifier[0] = 1;

        var fakeUser = new User(
            Guid.NewGuid(), 
            "test@email.com", 
            "testuser", 
            fakeSalt, 
            fakeVerifier
        );

        _userRepositoryMock
            .Setup(repo => repo.GetUserByUsernameOrEmailAsync("testuser", It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeUser);
        
        var validBase64A = Convert.ToBase64String(new byte[] { 2, 4, 6, 8 });
        var command = new LoginInitCommand("testuser", "test@email.com", validBase64A);
        
        var result = await _handler.Handle(command, CancellationToken.None);
        
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(Convert.ToBase64String(fakeSalt), result.Value.Salt);
        Assert.NotEqual(Guid.Empty, result.Value.SessionId);
        Assert.NotEmpty(result.Value.B);
        
        _sessionStoreMock.Verify(store => 
            store.SaveSession(It.IsAny<Guid>(), It.IsAny<Srp6Server>()), 
            Times.Once);
    }
}