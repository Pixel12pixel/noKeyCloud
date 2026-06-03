using Moq;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Application.Features.Admin.GenerateRegisterInvite;
using noKeyCloud.Domain.Entities;

namespace noKeyCloud.Application.UnitTests.Features.Admin.GenerateRegisterInvite;

public class GenerateRegisterInviteHandlerTests
{
    [Fact]
    public async Task Handle_ValidRequest_CreatesInviteAndReturnsCode()
    {
        var repoMock = new Mock<IRegisterInviteRepository>();
        
        RegisterInvite? capturedInvite = null;
        repoMock.Setup(x => x.AddAsync(It.IsAny<RegisterInvite>(), It.IsAny<CancellationToken>()))
            .Callback<RegisterInvite, CancellationToken>((i, ct) => capturedInvite = i)
            .Returns(Task.CompletedTask);

        var handler = new GenerateRegisterInviteHandler(repoMock.Object);
        var cmd = new GenerateRegisterInviteCommand(7);

        var result = await handler.Handle(cmd, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.False(string.IsNullOrWhiteSpace(result.Value));
        
        Assert.NotNull(capturedInvite);
        Assert.Equal(result.Value, capturedInvite.Code);
        
        var timeDifference = capturedInvite.ExpiresAt - DateTime.UtcNow;
        Assert.True(timeDifference != null && timeDifference.Value.TotalDays > 6.9 && timeDifference.Value.TotalDays < 7.1);
    }
}
