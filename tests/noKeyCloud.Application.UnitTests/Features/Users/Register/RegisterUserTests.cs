using Moq;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Application.Features.Users.Register;
using noKeyCloud.Domain.Entities;
using noKeyCloud.Application.Features.Folders; 

namespace noKeyCloud.Application.UnitTests.Features.Users.Register;

public class RegisterUserTests
{
    [Fact]
    public async Task Should_register_user_and_create_correct_root_folder()
    {
        var repoMock = new Mock<IUserRepository>();
        var folderRepoMock = new Mock<IFolderRepository>();

        User capturedUser = null;
        repoMock.Setup(x => x.CreateUser(It.IsAny<User>()))
            .Callback<User>(u => capturedUser = u)
            .Returns(Task.CompletedTask);

        var handler = new RegisterUserHandler(repoMock.Object, folderRepoMock.Object);
        var cmd = new RegisterUserCommand("mine", "minipaka", new byte[] { 75 }, new byte[] { 34 });

        var result = await handler.Handle(cmd, CancellationToken.None);
        
        Assert.True(result.IsSuccess);
        Assert.NotNull(capturedUser);

        var expectedRootFolderId = FolderIdHelper.GenerateRootFolderId(capturedUser.Id);

        folderRepoMock.Verify(x => x.AddFolder(
                It.Is<Folder>(f => 
                    f.Id == expectedRootFolderId && 
                    f.UserId == capturedUser.Id && 
                    f.ParentFolderId == null), 
                It.IsAny<CancellationToken>()), 
            Times.Once);
    }
}