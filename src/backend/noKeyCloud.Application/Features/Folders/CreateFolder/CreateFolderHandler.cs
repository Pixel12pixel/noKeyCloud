using MediatR;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Contracts.Folders;
using noKeyCloud.Domain.Entities;
using System.Security.Cryptography;
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


        var temporaryNameBytes = Encoding.UTF8.GetBytes(request.FolderName);
        var emptyKeyBytes = Array.Empty<byte>();

        if (request.ParentFolderId.HasValue && request.ParentFolderId.Value == Guid.Empty)
        {
            throw new ArgumentException("ParentFolderId cannot be Guid.Empty. Use null for root folders.", nameof(request.ParentFolderId));
        }

        var rootFolderId = GenerateRootFolderId(request.UserId);
        var parentFolderId = request.ParentFolderId ?? null;

        var now = DateTime.UtcNow;
        var folder = new Folder(
            id: Guid.NewGuid(),
            encryptedName: temporaryNameBytes,
            encryptedKey: emptyKeyBytes,
            createdAt: now,
            updatedAt: now,
            parentFolderId: parentFolderId,
            userId: request.UserId
        );


        var createdFolder = await folderRepository.AddFolder(folder, cancellationToken);

        // TODO: Temporarily convert the byte[] back to normal string name
        var responseName = Encoding.UTF8.GetString(createdFolder.EncryptedName);

        return new CreateFolderResponse(createdFolder.Id, responseName);
    }

    /// Generates a unique root folder ID based on the UserId.
    private static Guid GenerateRootFolderId(Guid userId)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes($"{userId}:root"));
        var guidBytes = new byte[16];
        Array.Copy(hash, guidBytes, guidBytes.Length);
        return new Guid(guidBytes);
    }
}