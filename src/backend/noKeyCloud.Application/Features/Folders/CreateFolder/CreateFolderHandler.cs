using MediatR;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Contracts.Folders;
using noKeyCloud.Domain.Entities;
using System.Text;

namespace noKeyCloud.Application.Features.Folders.CreateFolder;

public class CreateFolderHandler(IFolderRepository folderRepository)
    : IRequestHandler<CreateFolderCommand, CreateFolderResponse>
{
    public async Task<CreateFolderResponse> Handle(CreateFolderCommand request, CancellationToken cancellationToken)
    {
        if (request.ParentFolderId == null)
        {
            throw new Exception("ParentFolderId cannot be null.");
        }

        if (request.EncryptedName is null || request.EncryptedName.Length == 0)
        {
            throw new ArgumentException("Folder name cannot be empty.", nameof(request.EncryptedName));
        }

        if (request.ParentFolderId.HasValue && request.ParentFolderId.Value == Guid.Empty)
        {
            throw new ArgumentException("ParentFolderId cannot be Guid.Empty.", nameof(request.ParentFolderId));
        }
        
        var parentFolderId = request.ParentFolderId ?? null;

        var now = DateTime.UtcNow;
        var folder = new Folder(
            id: Guid.NewGuid(),
            encryptedName: request.EncryptedName,
            encryptedKey: request.EncryptedKey,
            createdAt: now,
            updatedAt: now,
            parentFolderId: parentFolderId,
            userId: request.UserId
        );


        var createdFolder = await folderRepository.AddFolder(folder, cancellationToken);

        return new CreateFolderResponse(createdFolder.Id);
    }
}