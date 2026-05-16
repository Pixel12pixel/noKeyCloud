using Moq;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Application.Features.Users.Register;

namespace noKeyCloud.Application.UnitTests.Features.Users.Register;

public class RegisterUserTests
{
    [Fact]
    public async Task Should_register_user()
    {
        var repoMock = new Mock<IUserRepository>();
        var folderRepoMock = new Mock<IFolderRepository>();
        var handler = new RegisterUserHandler(repoMock.Object, folderRepoMock.Object);
        var cmd = new RegisterUserCommand("mine", "minipaka", new byte[] { 75 }, new byte[] { 34 });

        var result = await handler.Handle(cmd,  CancellationToken.None);
        
        Assert.True(result.IsSuccess);
    }
}