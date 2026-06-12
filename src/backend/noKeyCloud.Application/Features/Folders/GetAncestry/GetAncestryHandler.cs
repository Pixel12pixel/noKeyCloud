using MediatR;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Application.Features.Folders.GetAncestry;
using noKeyCloud.Contracts.Common;
using noKeyCloud.Contracts.Folders;

namespace noKeyCloud.Application.Features.Folders.GetAncestry;

public class GetAncestryHandler(IFolderRepository folderRepository)
    : IRequestHandler<GetAncestryQuery, Result<GetAncestryResponse>>
{
    public async Task<Result<GetAncestryResponse>> Handle(GetAncestryQuery request, CancellationToken cancellationToken)
    {
        var ancestryChain = new List<FolderAncestryItem>();
        var currentFolderId = request.FolderId;

        while (currentFolderId != Guid.Empty)
        {
            var folder = await folderRepository.GetByIdAndUserIdAsync(
                currentFolderId,
                request.UserId,
                cancellationToken);

            if (folder == null)
            {
                return Result<GetAncestryResponse>.Failure("Folder not found.");
            }

            ancestryChain.Insert(0,
                new FolderAncestryItem(
                    folder.Id,
                    folder.EncryptedName,
                    folder.EncryptedKey));

            if (folder.ParentFolderId == null)
            {
                break;
            }

            currentFolderId = folder.ParentFolderId.Value;
        }

        return Result<GetAncestryResponse>.Success(new GetAncestryResponse(ancestryChain));
    }
}