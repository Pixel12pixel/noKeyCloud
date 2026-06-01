using Moq;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Application.Features.Files.CreateFile;
using noKeyCloud.Application.Features.Files.UploadFile;
using noKeyCloud.Domain.Entities;

namespace noKeyCloud.Application.UnitTests.Features.File.UploadFile;

public class UploadFileTests
{
    private readonly Mock<IFileRepository> _fileRepositoryMock;
    private  readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IFolderRepository> _folderRepositoryMock;
    private readonly UploadFileHandler _handler;
    
    public UploadFileTests()
    {
        _fileRepositoryMock = new Mock<IFileRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _folderRepositoryMock = new Mock<IFolderRepository>();
        
        _handler = new UploadFileHandler(_fileRepositoryMock.Object,  _userRepositoryMock.Object,  _folderRepositoryMock.Object);
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
        
        _userRepositoryMock
            .Setup(repo => repo.GetUserByUserId(fakeUser.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeUser);
        
        _folderRepositoryMock
            .Setup(repo => repo.GetFolderByFolderId(fakeFolder.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeFolder);
        
        _fileRepositoryMock
            .Setup(repo => repo.CreateFile(It.IsAny<noKeyCloud.Domain.Entities.File>(), It.IsAny<CancellationToken>(), It.Is<byte[]>(content => content == null)))
            .Returns(Task.CompletedTask);
        
        var command = new UploadFileCommand(fakeUser.Id, [] ,"MimeType",0,[],[],fakeFolder.Id.ToString(),[]);
        
        var result = await _handler.Handle(command, CancellationToken.None);
        
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.NotEqual(Guid.Empty, result.Value.FileId);
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
        
        _userRepositoryMock
            .Setup(repo => repo.GetUserByUserId(fakeUser.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeUser);
        
        _folderRepositoryMock
            .Setup(repo => repo.GetFolderByFolderId(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Folder?)null);
        
        _fileRepositoryMock
            .Setup(repo => repo.CreateFile(It.IsAny<noKeyCloud.Domain.Entities.File>(), It.IsAny<CancellationToken>(), It.Is<byte[]>(content => content == null)))
            .Returns(Task.CompletedTask);
        
        var command = new UploadFileCommand(fakeUser.Id, [] ,"MimeType",0,[],[],fakeFolder.Id.ToString(),[]);
        
        var result = await _handler.Handle(command, CancellationToken.None);
        
        Assert.False(result.IsSuccess);
        Assert.Equal("Folder not found", result.Error);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ShouldReturnFalse()
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

        _userRepositoryMock
            .Setup(repo => repo.GetUserByUserId(fakeUser.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        _folderRepositoryMock
            .Setup(repo => repo.GetFolderByFolderId(fakeFolder.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeFolder);

        var command = new UploadFileCommand(fakeUser.Id, [] ,"MimeType",0,[],[],fakeFolder.Id.ToString(),[]);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("User not found", result.Error);
    }

    [Fact]
    public async Task Handle_WhenFolderIdFormatIsInvalid_ShouldReturnFalse()
    {
        var userId = Guid.NewGuid();

        var command = new UploadFileCommand(Guid.NewGuid(), [] ,"MimeType",0,[],[],"not-guid",[]);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Wrong id format", result.Error);
    }
}