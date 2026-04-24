using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Moq;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Application.Features.Folders.Commands.CreateFolder;
using noKeyCloud.Domain.Entities;

namespace noKeyCloud.Application.UnitTests.Features.Folders.Commands;

public class CreateFolderHandlerTests
{
    private readonly Mock<IFolderRepository> _folderRepositoryMock;
    private readonly CreateFolderHandler _handler;

    public CreateFolderHandlerTests()
    {
        _folderRepositoryMock = new Mock<IFolderRepository>();
        _handler = new CreateFolderHandler(_folderRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateFolder_AndReturnIdAndName()
    {
        var userId = Guid.NewGuid();
        var command = new CreateFolderCommand(
            UserId: userId,
            FolderName: "New Project",
            ParentFolderId: null
        );
        
        _folderRepositoryMock
            .Setup(repo => repo.AddFolder(It.IsAny<Folder>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Folder f, CancellationToken _) => f);
        
        var result = await _handler.Handle(command, CancellationToken.None);
        
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("New Project", result.Name);
        
        var inputBytes = Encoding.UTF8.GetBytes($"{userId}:root");
        var hashBytes = SHA256.HashData(inputBytes);
        var expectedGuidBytes = new byte[16];
        Array.Copy(hashBytes, expectedGuidBytes, 16);
        var expectedRootFolderId = new Guid(expectedGuidBytes);

        _folderRepositoryMock.Verify(
            x => x.AddFolder(
                It.Is<Folder>(f => 
                    f.UserId == command.UserId && 
                    Encoding.UTF8.GetString(f.EncryptedName) == command.FolderName && 
                    f.ParentFolderId == expectedRootFolderId),
                It.IsAny<CancellationToken>()), 
            Times.Once);
    }
    
    [Fact]
    public async Task Handle_WithProvidedParentFolderId_ShouldMapParentFolderIdCorrectly()
    {
        var parentId = Guid.NewGuid();
        var command = new CreateFolderCommand(
            UserId: Guid.NewGuid(),
            FolderName: "Sub Folder",
            ParentFolderId: parentId
        );
        
        _folderRepositoryMock
            .Setup(repo => repo.AddFolder(It.IsAny<Folder>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Folder f, CancellationToken _) => f);
        
        var result = await _handler.Handle(command, CancellationToken.None);
        
        Assert.NotNull(result);
        _folderRepositoryMock.Verify(
            x => x.AddFolder(
                It.Is<Folder>(f => f.ParentFolderId == parentId), 
                It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithGuidEmptyParentFolderId_ShouldThrowArgumentException()
    {
        var command = new CreateFolderCommand(
            UserId: Guid.NewGuid(),
            FolderName: "Valid Folder",
            ParentFolderId: Guid.Empty
        );
        
        var act = async () => await _handler.Handle(command, CancellationToken.None);
        
        var exception = await Assert.ThrowsAsync<ArgumentException>(act);
        Assert.Contains("cannot be Guid.Empty", exception.Message);
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrowsDbUpdateException_ShouldPropagateException()
    {
        var command = new CreateFolderCommand(
            UserId: Guid.NewGuid(),
            FolderName: "Failed Folder",
            ParentFolderId: null
        );
        
        var innerException = new Exception("Database constraint violation");
        var dbException = new DbUpdateException("An error occurred while updating the entries.", innerException);

        _folderRepositoryMock
            .Setup(repo => repo.AddFolder(It.IsAny<Folder>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(dbException);
        
        var act = async () => await _handler.Handle(command, CancellationToken.None);
        
        var exception = await Assert.ThrowsAsync<DbUpdateException>(act);
        Assert.Equal(dbException.Message, exception.Message);
        Assert.Same(innerException, exception.InnerException);
    }

    [Fact]
    public async Task Handle_WhenCancellationTokenIsCancelled_ShouldThrowOperationCanceledException()
    {
        var command = new CreateFolderCommand(
            UserId: Guid.NewGuid(),
            FolderName: "Canceled Folder",
            ParentFolderId: null
        );
        
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();
        
        _folderRepositoryMock
            .Setup(repo => repo.AddFolder(It.IsAny<Folder>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException(cancellationTokenSource.Token));
        
        var act = async () => await _handler.Handle(command, cancellationTokenSource.Token);
        
        await Assert.ThrowsAsync<OperationCanceledException>(act);
    }
}