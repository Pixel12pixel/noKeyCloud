using System.Text;
using MediatR;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Contracts.Common;
using noKeyCloud.Domain.Entities;
using noKeyCloud.Application.Features.Folders;

namespace noKeyCloud.Application.Features.Users.Register;

public class RegisterUserHandler(IUserRepository userRepository, IFolderRepository folderRepository, IRegisterInviteRepository registerInviteRepository)
    : IRequestHandler<RegisterUserCommand, Result>
{
    public async Task<Result> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        bool isFirstUser = !await userRepository.HasAnyUsersAsync(cancellationToken);
        RegisterInvite? validInvite = null;
        
        
        if (!isFirstUser)
        {
            if (string.IsNullOrWhiteSpace(request.RegisterInviteCode))
                return Result.Failure("An invite code is required to register.");
            
            validInvite = await registerInviteRepository.GetByCodeAsync(request.RegisterInviteCode, cancellationToken);
            
            if (validInvite == null || !validInvite.IsValid())
                return Result.Failure("Invalid, expired, or already used invite code.");
        }
        
        try
        {
            var user = new User(Guid.NewGuid(), request.Email, request.Username, request.Salt, request.Verifier, isAdmin: isFirstUser);


            if (user is null)
            {
                return Result.Failure("Failed to create user.");
            }

            var temporaryNameBytes = Encoding.UTF8.GetBytes("home-" + user.Username);

            var emptyKeyBytes = Array.Empty<byte>();
            var now = DateTime.UtcNow;
            
            var rootFolderId = FolderIdHelper.GenerateRootFolderId(user.Id);

            var rootFolder = new Folder(
              id: rootFolderId,
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

            if (validInvite != null)
            {
                validInvite.MarkAsUsed();
                await registerInviteRepository.UpdateAsync(validInvite, cancellationToken);
            }

        }
        catch (Exception e)
        {
            return Result.Failure(e.Message);
        }

        return Result.Success();
    }
}