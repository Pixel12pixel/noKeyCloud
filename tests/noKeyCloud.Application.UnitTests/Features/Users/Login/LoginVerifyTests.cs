using System.Numerics;
using Moq;
using noKeyCloud.Application.Abstractions.Services;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Domain.Entities;
using noKeyCloud.Application.Features.Users.LoginVerify;
using noKeyCloud.Application.Security;

namespace noKeyCloud.Application.UnitTests.Features.Users.Login;

public class LoginVerifyTests
{
    private readonly Mock<ISrpSessionStore> _sessionStoreMock;
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly LoginVerifyCommandHandler _handler;
    private readonly Mock<IRefreshTokenProvider> _refreshTokenProviderMock;
    private readonly Mock<IFolderRepository> _folderRepositoryMock;

    public LoginVerifyTests()
    {
        _sessionStoreMock = new Mock<ISrpSessionStore>();

        _refreshTokenProviderMock = new Mock<IRefreshTokenProvider>();
        
        _refreshTokenProviderMock.Setup(x => x.GenerateRefreshToken())
            .Returns("dummy-refresh-token");

        _jwtServiceMock = new Mock<IJwtService>();
        _jwtServiceMock
            .Setup(x => x.JwtTokenService(It.IsAny<Guid?>()))
            .ReturnsAsync("fake-jwt-token");
            
        _folderRepositoryMock = new Mock<IFolderRepository>();
        
        _handler = new LoginVerifyCommandHandler(_jwtServiceMock.Object, _sessionStoreMock.Object, _refreshTokenProviderMock.Object, _folderRepositoryMock.Object);

    }

    [Fact]
    public async Task Handle_WhenSessionNotFound_ShouldReturnFailure()
    {
        var sessionId = Guid.NewGuid();

        _sessionStoreMock
            .Setup(x => x.GetSession(sessionId))
            .Returns((SrpSession?)null);

        var command = new LoginVerifyCommand(sessionId.ToString(), "1234567890");
        
        var result = await _handler.Handle(command, CancellationToken.None);
        
        Assert.False(result.IsSuccess);
        Assert.Equal("Session not found.", result.Error);
    }
    
    [Fact]
    public async Task Handle_WhenM1IsValid_ShouldReturnSuccess()
    {
        var salt = new byte[] { 1, 2, 3 };
        var username = "user";
        
        var sessionId = Guid.NewGuid();
        var testUserId = Guid.NewGuid();
        var testRootFolderId = Guid.NewGuid();
        
        var mockFolder = new Folder(
            testRootFolderId,
            "Root"u8.ToArray(),
            testUserId.ToByteArray(),
            DateTime.UtcNow,
            DateTime.UtcNow,
            null,
            Guid.Empty
        );

        var srpServer = new NativeSrpServer();
        var A = new BigInteger(123);
        var B = new BigInteger(456);
        var S = new BigInteger(789);
        
        var expectedM1Bytes = srpServer.ComputeExpectedClientProof(username, salt, A, B, S);
        var M1 = Convert.ToBase64String(expectedM1Bytes);

        var session = new SrpSession 
        {
            Username = username,
            Salt = salt,
            A = A,
            B = B,
            S = S
        };

        _folderRepositoryMock.Setup(repo => repo.GetUserHomeFolder(testUserId, default)).ReturnsAsync(mockFolder);
        
        _sessionStoreMock
            .Setup(x => x.GetSession(sessionId))
            .Returns(session);
        
        _sessionStoreMock
            .Setup(x => x.GetUserId(sessionId))
            .Returns(testUserId);

        _sessionStoreMock
            .Setup(x => x.DeleteSession(sessionId))
            .Returns(true);

        var command = new LoginVerifyCommand(
            sessionId.ToString(),
            M1
        );

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        
        Assert.NotNull(result.Value);
        Assert.Equal("dummy-refresh-token", result.Value!.RefreshToken);
        Assert.NotNull(result.Value.JwtToken);
        Assert.Equal(testRootFolderId.ToString(), result.Value!.ResponsePayload.RootFolderId);
        
        _refreshTokenProviderMock.Verify(x => x.StoreRefreshTokenAsync(
            It.IsAny<Guid>(),
            "dummy-refresh-token",
            It.IsAny<TimeSpan>(),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }
    
    [Fact]
    public async Task Handle_WhenM1IsInvalid_ShouldReturnFailure()
    {
        var sessionId = Guid.NewGuid();

        var session = new SrpSession 
        {
            Username = "user",
            Salt = new byte[] { 1, 2, 3 },
            A = new BigInteger(123),
            B = new BigInteger(456),
            S = new BigInteger(789)
        };

        _sessionStoreMock
            .Setup(x => x.GetSession(sessionId))
            .Returns(session);

        var fakeM1 = Convert.ToBase64String(new byte[] { 9, 9, 9 });
        
        var command = new LoginVerifyCommand(sessionId.ToString(), fakeM1);
        
        var result = await _handler.Handle(command, CancellationToken.None);
        
        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid credentials.", result.Error);
    }
}