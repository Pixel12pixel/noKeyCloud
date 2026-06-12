using MediatR;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Application.Features.Folders;
using noKeyCloud.Contracts.Common;
using noKeyCloud.Contracts.User;

namespace noKeyCloud.Application.Features.Users.GetMe;

public class GetMeQueryHandler(IUserRepository userRepository, IFolderRepository folderRepository) : IRequestHandler<GetMeQuery, Result<GetMeResponse>>
{
    public async Task<Result<GetMeResponse>> Handle(GetMeQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserByUserId(request.UserId, cancellationToken);
        
        if (user == null)
        {
            return Result<GetMeResponse>.Failure("User not found.");
        }
        
        var rootFolderId = FolderIdHelper.GenerateRootFolderId(user.Id);
        
        var folder = await folderRepository.GetFolderByFolderId(rootFolderId, cancellationToken);
        
        if (folder == null)
        {
            return Result<GetMeResponse>.Failure("Folder not found.");
        }

        var response = new GetMeResponse(
            user.Id.ToString(),
            user.Username,
            user.Email,
            rootFolderId.ToString(),
            folder.EncryptedKey,
            IsAdmin: user.IsAdmin
        );

        return Result<GetMeResponse>.Success(response);
    }
}