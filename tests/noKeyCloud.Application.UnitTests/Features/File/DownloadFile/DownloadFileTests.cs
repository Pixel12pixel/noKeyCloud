using MediatR;
using Moq;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Application.Features.Files.DownloadFile;
using noKeyCloud.Contracts.File;

namespace noKeyCloud.Application.UnitTests.Features.File.DownloadFile;

public class DownloadFileTests
{
    private readonly Mock<IFileRepository> _fileRepositoryMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly DownloadFileHandler _handler;

    public DownloadFileTests()
    {
        _fileRepositoryMock = new Mock<IFileRepository>();
        _mediatorMock = new Mock<IMediator>();

        _handler = new DownloadFileHandler(_fileRepositoryMock.Object, _mediatorMock.Object);
    }

    [Fact]
    public async Task Handle_WhenValidCredentials_ShouldReturnFileData()
    {
        var userId = Guid.NewGuid();
        var fileId = Guid.NewGuid();
        var parentFolderId = Guid.NewGuid();
        var fileContent = new byte[] { 1, 2, 3 };
        var encryptedName = new byte[] { 10, 20 };
        var encryptedKey = new byte[] { 30, 40 };
        var checksum = new byte[] { 50, 60 };
        var mimeType = "application/octet-stream";
        var file = new noKeyCloud.Domain.Entities.File(
            fileId,
            encryptedName,
            mimeType,
            sizeBytes: fileContent.Length,
            storagePath: "path",
            encryptedKey,
            checksum,
            parentFolderId,
            userId);

        _fileRepositoryMock
            .Setup(repo => repo.GetFileById(fileId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((file, fileContent));

        var command = new DownloadFileCommand(userId, fileId);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(fileId, result.Value.fileId);
        Assert.Equal(fileContent, result.Value.fileContent);
        Assert.Equal(mimeType, result.Value.MimeType);
        Assert.Equal(encryptedName, result.Value.EncryptedName);
        Assert.Equal(encryptedKey, result.Value.EncryptedKey);
        Assert.Equal(checksum, result.Value.Checksum);
    }

    [Fact]
    public async Task Handle_WhenIdIsInvalid_ShouldThrowError()
    {
        var userId = Guid.NewGuid();
        var fileId = Guid.NewGuid();

        _fileRepositoryMock
            .Setup(repo => repo.GetFileById(fileId, userId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("File not found"));

        var command = new DownloadFileCommand(userId, fileId);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
