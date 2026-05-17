using MediatR;
using noKeyCloud.Application.Features.Folders.Commands.CreateFolder;

namespace noKeyCloud.Application.Features.Folders.CreateFolder;

public record CreateFolderCommand(
    Guid UserId,
    string FolderName,
    Guid? ParentFolderId) : IRequest<CreateFolderResponse>;
