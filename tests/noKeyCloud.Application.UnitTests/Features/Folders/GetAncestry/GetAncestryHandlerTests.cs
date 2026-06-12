using Moq;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Application.Features.Folders.GetAncestry;
using noKeyCloud.Domain.Entities;

namespace noKeyCloud.Application.UnitTests.Features.Folders.GetAncestry;

public class GetAncestryHandlerTests
{
    private readonly Mock<IFolderRepository> _folderRepositoryMock;
    private readonly GetAncestryHandler _handler;

    public GetAncestryHandlerTests()
    {
        _folderRepositoryMock = new Mock<IFolderRepository>();
        _handler = new GetAncestryHandler(_folderRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenFolderDoesNotExist()
    {
        var query = new GetAncestryQuery(
            Guid.NewGuid(),
            Guid.NewGuid());

        _folderRepositoryMock
            .Setup(x => x.GetByIdAndUserIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Folder?)null);
        
        var result = await _handler.Handle(query, CancellationToken.None);
        
        Assert.False(result.IsSuccess);
        Assert.Equal("Folder not found.", result.Error);
    }
}