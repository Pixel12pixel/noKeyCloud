using System.Text;
using Moq;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Application.Features.Folders;
using noKeyCloud.Application.Features.Users.GetMe;
using noKeyCloud.Domain.Entities;

namespace noKeyCloud.Application.UnitTests.Features.Users;

public class GetMeQueryHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IFolderRepository> _folderRepositoryMock;
    private readonly GetMeQueryHandler _handler;

    public GetMeQueryHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _folderRepositoryMock = new Mock<IFolderRepository>();

        _handler = new GetMeQueryHandler(_userRepositoryMock.Object,  _folderRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_UserExists_ReturnsSuccessAndUserResponse()
    {
        var userId = Guid.NewGuid();
        var rootFolderId = FolderIdHelper.GenerateRootFolderId(userId);
        var query = new GetMeQuery(userId);

        var user = new User(
            userId,
            "test@email.com",
            "testUser",
            new byte[] { 1 },
            new byte[] { 2 },
            new byte[] { 3 },
            new byte[] { 5 },
            isAdmin: true);

        var folder = new Folder(
            id: rootFolderId,
            encryptedName: Encoding.UTF8.GetBytes("EncryptedFolder"),
            encryptedKey: Encoding.UTF8.GetBytes("EncryptedKey"),
            createdAt: DateTime.UtcNow,
            updatedAt: DateTime.UtcNow,
            parentFolderId: null,
            userId: userId
        );

        _userRepositoryMock
            .Setup(x => x.GetUserByUserId(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _folderRepositoryMock
            .Setup(x => x.GetFolderByFolderId(rootFolderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(folder);
        
        var result = await _handler.Handle(
            query,
            CancellationToken.None);
        
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(userId.ToString(), result.Value.UserId);
        Assert.Equal("testUser", result.Value.Username);
        Assert.Equal("test@email.com", result.Value.Email);
        Assert.Equal(rootFolderId.ToString(), result.Value.RootFolderId);
        Assert.True(result.Value.IsAdmin);
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