using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Application.Features.Folders.ListContent;
using noKeyCloud.Domain.Entities;
using Xunit;

public class ListContentHandlerTests
{
    [Fact]
    public async Task Handle_WhenRepositoryReturnsFilesAndFolders_ShouldMapResponseCorrectly()
    {
        // Arrange
        var repoMock = new Mock<IFolderRepository>();
        var handler = new ListContentHandler(repoMock.Object);

        var userId = Guid.NewGuid();
        var folderId = Guid.NewGuid();

        var parentFolderIdForFolder = Guid.Empty;

        var now = DateTime.UtcNow;

        var files = new List<noKeyCloud.Domain.Entities.File>
        {
            new noKeyCloud.Domain.Entities.File(
                id: Guid.NewGuid(),
                encryptedName: Encoding.UTF8.GetBytes("enc_file_1"),
                mimeType: "text/plain",
                sizeBytes: 123L,
                storagePath: "/fake/path/file1",
                encryptedKey: Encoding.UTF8.GetBytes("key1"),
                checksum: Encoding.UTF8.GetBytes("sha1"),
                parentFolderId: folderId,
                userId: userId
            )
        };

        var folders = new List<Folder>
        {
            new Folder(
                id: Guid.NewGuid(),
                encryptedName: Encoding.UTF8.GetBytes("enc_folder_1"),
                encryptedKey: Encoding.UTF8.GetBytes("fkey1"),
                createdAt: now,
                updatedAt: now,
                parentFolderId: parentFolderIdForFolder,
                userId: userId
            )
        };

        repoMock
            .Setup(r => r.GetFilesOrContentAsync(folderId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((files, folders));

        var query = new ListContentQuery(folderId, userId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value.Files);
        Assert.Single(result.Value.Folders);

        
        repoMock.Verify(
            r => r.GetFilesOrContentAsync(folderId, userId, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}