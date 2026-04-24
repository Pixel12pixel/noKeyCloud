using MediatR;

namespace noKeyCloud.Application.Features.Folders.Commands.CreateFolder;

public record CreateFolderCommand(
    Guid UserId,
    string FolderName,
    Guid? ParentFolderId) : IRequest<CreateFolderResponse>;
