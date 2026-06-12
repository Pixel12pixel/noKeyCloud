using MediatR;
using noKeyCloud.Contracts.Folders;

namespace noKeyCloud.Application.Features.Folders.CreateFolder;

public record CreateFolderCommand(
    Guid UserId,
    byte[] EncryptedName,
    byte[] EncryptedKey,
    Guid? ParentFolderId) : IRequest<CreateFolderResponse>;
