using Moq;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Application.Features.Users.CreateFile;
using noKeyCloud.Domain.Entities;

namespace noKeyCloud_apiUnitTests.Features.Users.File;

public class CreateFileTests
{
    private readonly Mock<IFileRepository> _fileRepositoryMock;
    private readonly CreateFileHandler _handler;
    
    public CreateFileTests()
    {
        _fileRepositoryMock = new Mock<IFileRepository>();
        
        _handler = new CreateFileHandler(_fileRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WhenFileCreated_ShouldReturnSuccess()
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

        var fakeFolder = new Folder(
            Guid.NewGuid(),
            [],
            [],
            DateTime.Now,
            DateTime.Now,
            Guid.NewGuid(),
            fakeUser.Id
        );
        
        _fileRepositoryMock
            .Setup(repo => repo.FileExists(It.IsAny<byte[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        _fileRepositoryMock
            .Setup(repo => repo.GetUserByUserId(fakeUser.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeUser);
        
        _fileRepositoryMock
            .Setup(repo => repo.GetFolderByFolderId(fakeFolder.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeFolder);
        
        _fileRepositoryMock
            .Setup(repo => repo.CreateFile(It.IsAny<noKeyCloud.Domain.Entities.File>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        var command = new CreateFileCommand(fakeUser.Id.ToString(), "testFileName", fakeFolder.Id.ToString());
        
        var result = await _handler.Handle(command, CancellationToken.None);
        
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("testFileName", result.Value.FileName);
        Assert.NotEqual(Guid.Empty, result.Value.FileId);
        Assert.Equal(0, result.Value.FileSize);
    }
    
    [Fact]
    public async Task Handle_WhenFileIdIsInvalid_ShouldReturnFalse()
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

        var fakeFolder = new Folder(
            Guid.NewGuid(),
            [],
            [],
            DateTime.Now,
            DateTime.Now,
            Guid.NewGuid(),
            fakeUser.Id
        );
        
        _fileRepositoryMock
            .Setup(repo => repo.FileExists(It.IsAny<byte[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        _fileRepositoryMock
            .Setup(repo => repo.GetUserByUserId(fakeUser.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeUser);
        
        _fileRepositoryMock
            .Setup(repo => repo.GetFolderByFolderId(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Folder?)null);
        
        _fileRepositoryMock
            .Setup(repo => repo.CreateFile(It.IsAny<noKeyCloud.Domain.Entities.File>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        var command = new CreateFileCommand(fakeUser.Id.ToString(), "testFileName", fakeFolder.Id.ToString());
        
        var result = await _handler.Handle(command, CancellationToken.None);
        
        Assert.False(result.IsSuccess);
        Assert.Equal("Folder not found", result.Error);
    }
    
    [Fact]
    public async Task Handle_WhenFileExist_ShouldReturnFasle()
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

        var fakeFolder = new Folder(
            Guid.NewGuid(),
            [],
            [],
            DateTime.Now,
            DateTime.Now,
            Guid.NewGuid(),
            fakeUser.Id
        );
        
        _fileRepositoryMock
            .Setup(repo => repo.FileExists(It.IsAny<byte[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        _fileRepositoryMock
            .Setup(repo => repo.GetUserByUserId(fakeUser.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeUser);
        
        _fileRepositoryMock
            .Setup(repo => repo.GetFolderByFolderId(fakeFolder.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeFolder);
        
        _fileRepositoryMock
            .Setup(repo => repo.CreateFile(It.IsAny<noKeyCloud.Domain.Entities.File>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        var command = new CreateFileCommand(fakeUser.Id.ToString(), "testFileName", fakeFolder.Id.ToString());
        
        var result = await _handler.Handle(command, CancellationToken.None);
        
        Assert.False(result.IsSuccess);
        Assert.Equal("File already exists", result.Error);
    }
}