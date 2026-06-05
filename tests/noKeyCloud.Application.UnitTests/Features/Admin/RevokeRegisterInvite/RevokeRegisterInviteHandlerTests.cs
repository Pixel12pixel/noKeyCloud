using Moq;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Application.Features.Admin.RevokeRegisterInvite;
using noKeyCloud.Domain.Entities;

namespace noKeyCloud.Application.UnitTests.Features.Admin.RevokeRegisterInvite;

public class RevokeRegisterInviteHandlerTests
{
    [Fact]
    public async Task Handle_InviteExists_DeletesInviteAndReturnsSuccess()
    {
        var repoMock = new Mock<IRegisterInviteRepository>();
        var inviteId = Guid.NewGuid();
        var invite = new RegisterInvite(inviteId, "aB3dE9fG", DateTime.UtcNow, DateTime.UtcNow.AddHours(24));
        
        repoMock.Setup(x => x.GetByIdAsync(inviteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invite);
            
        repoMock.Setup(x => x.DeleteAsync(invite, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new RevokeRegisterInviteHandler(repoMock.Object);
        var cmd = new RevokeRegisterInviteCommand(inviteId);
        
        var result = await handler.Handle(cmd, CancellationToken.None);
        
        Assert.True(result.IsSuccess);
        repoMock.Verify(x => x.DeleteAsync(invite, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_InviteDoesNotExist_ReturnsFailure()
    {
        var repoMock = new Mock<IRegisterInviteRepository>();
        var inviteId = Guid.NewGuid();
        
        repoMock.Setup(x => x.GetByIdAsync(inviteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((RegisterInvite?)null);

        var handler = new RevokeRegisterInviteHandler(repoMock.Object);
        var cmd = new RevokeRegisterInviteCommand(inviteId);
        
        var result = await handler.Handle(cmd, CancellationToken.None);
        
        Assert.False(result.IsSuccess);
        Assert.Equal("Invite not found.", result.Error);
        
        repoMock.Verify(x => x.DeleteAsync(It.IsAny<RegisterInvite>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}