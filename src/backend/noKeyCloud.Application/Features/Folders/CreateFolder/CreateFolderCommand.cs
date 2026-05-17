using MediatR;
using noKeyCloud.Contracts.Folders;

namespace noKeyCloud.Application.Features.Folders.CreateFolder;

public record CreateFolderCommand(
    Guid UserId,
    string FolderName,
    Guid? ParentFolderId) : IRequest<CreateFolderResponse>;
