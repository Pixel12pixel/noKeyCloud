using Moq;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Application.Features.Admin.GetActiveRegisterInvites;
using noKeyCloud.Domain.Entities;

namespace noKeyCloud.Application.UnitTests.Features.Admin.GetActiveRegisterInvites;

public class GetActiveRegisterInvitesHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsMappedActiveInvites()
    {
        var repoMock = new Mock<IRegisterInviteRepository>();
        
        var creationTime1 = DateTime.UtcNow.AddMinutes(-10);
        var creationTime2 = DateTime.UtcNow.AddMinutes(-5);
        
        var invites = new List<RegisterInvite>
        {
            new RegisterInvite(Guid.NewGuid(), "aB3dE9fG", creationTime1, DateTime.UtcNow.AddHours(24)),
            new RegisterInvite(Guid.NewGuid(), "xY7zQ2wP", creationTime2, null)
        };

        repoMock.Setup(x => x.GetActiveInvitesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(invites);

        var handler = new GetActiveRegisterInvitesHandler(repoMock.Object);
        var query = new GetActiveRegisterInvitesQuery();
        
        var result = await handler.Handle(query, CancellationToken.None);
        
        Assert.NotNull(result);
        var resultList = result.ToList();
        
        Assert.Equal(2, resultList.Count);
        
        Assert.Equal(invites[0].Id, resultList[0].Id);
        Assert.Equal(invites[0].Code, resultList[0].Code);
        Assert.Equal(invites[0].CreatedAt, resultList[0].CreatedAt);
        Assert.Equal(invites[0].ExpiresAt, resultList[0].ExpiresAt);
        
        Assert.Equal(invites[1].Id, resultList[1].Id);
        Assert.Equal(invites[1].Code, resultList[1].Code);
        Assert.Equal(invites[1].CreatedAt, resultList[1].CreatedAt);
        Assert.Equal(invites[1].ExpiresAt, resultList[1].ExpiresAt);
    }
}