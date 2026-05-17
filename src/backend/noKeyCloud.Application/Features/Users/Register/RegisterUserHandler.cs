using MediatR;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Contracts.Common;
using noKeyCloud.Domain.Entities;
using System.Text;

namespace noKeyCloud.Application.Features.Users.Register;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Result>
{

    private readonly IUserRepository _userRepository;
    private readonly IFolderRepository _folderRepository;

    public RegisterUserHandler(IUserRepository userRepository, IFolderRepository folderRepository)
    {
        _userRepository = userRepository;
        _folderRepository = folderRepository;
    }

    public async Task<Result> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = new User(Guid.NewGuid(), request.Email, request.Username, request.Salt, request.Verifier);

            var temporaryNameBytes = Encoding.UTF8.GetBytes("home-" + user.Username);
            var emptyKeyBytes = Array.Empty<byte>();
            var now = DateTime.UtcNow;


            var rootFolder = new Folder(
            id: Guid.NewGuid(),
            encryptedName: temporaryNameBytes,
            encryptedKey: emptyKeyBytes,
            createdAt: now,
            updatedAt: now,
            parentFolderId: null,
            userId: user.Id
        );


            await _userRepository.CreateUser(user);
            await _folderRepository.AddFolder(rootFolder);
        }
        catch (Exception e)
        {
            return Result.Failure(e.Message);
        }

        return Result.Success();
    }
}