using MediatR;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Contracts.Common;
using noKeyCloud.Domain.Entities;
using noKeyCloud.Application.Features.Folders;

namespace noKeyCloud.Application.Features.Users.Register;

public class RegisterUserHandler(IUserRepository userRepository, IFolderRepository folderRepository)
    : IRequestHandler<RegisterUserCommand, Result>
{
    public async Task<Result> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = new User(Guid.NewGuid(), request.Email, request.Username, request.Salt, request.Verifier);


            if (user is null)
            {
                return Result.Failure("Failed to create user.");
            }

            var temporaryNameBytes = Encoding.UTF8.GetBytes("home-" + user.Username);

            var emptyKeyBytes = Array.Empty<byte>();
            var now = DateTime.UtcNow;
            
            var rootFolderId = FolderIdHelper.GenerateRootFolderId(user.Id);

            var rootFolder = new Folder(

            id: Guid.NewGuid(),
            encryptedName: temporaryNameBytes,
            encryptedKey: emptyKeyBytes,
            createdAt: now,
            updatedAt: now,
            parentFolderId: null,
            userId: user.Id
        );
            
        // TODO: wrap these two calls in a database transaction so that if the folder creation fails, the user isn't left in a broken state without a home folder.
        await userRepository.CreateUser(user);
        await folderRepository.AddFolder(rootFolder, cancellationToken);

        }
        catch (Exception e)
        {
            return Result.Failure(e.Message);
        }

        return Result.Success();
    }
}