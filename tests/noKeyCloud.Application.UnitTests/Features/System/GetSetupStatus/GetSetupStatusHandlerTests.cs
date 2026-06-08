using Moq;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Application.Features.System.GetSetupStatus;
using Xunit;
using System.Threading;
using System.Threading.Tasks;

namespace noKeyCloud.Application.UnitTests.Features.System.GetSetupStatus;

public class GetSetupStatusHandlerTests
{
    [Fact]
    public async Task Handle_WhenUsersExist_ReturnsFalse()
    {
        var repoMock = new Mock<IUserRepository>();
        repoMock.Setup(x => x.HasAnyUsersAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new GetSetupStatusHandler(repoMock.Object);
        var query = new GetSetupStatusQuery();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task Handle_WhenNoUsersExist_ReturnsTrue()
    {
        var repoMock = new Mock<IUserRepository>();
        repoMock.Setup(x => x.HasAnyUsersAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new GetSetupStatusHandler(repoMock.Object);
        var query = new GetSetupStatusQuery();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result);
    }
}

