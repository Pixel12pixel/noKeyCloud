using MediatR;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Contracts.Common;
using noKeyCloud.Contracts.File;
using System.Text;
using File = noKeyCloud.Domain.Entities.File;

namespace noKeyCloud.Application.Features.Files.CreateFile;

public class CreateFileHandler(IFileRepository fileRepository, IUserRepository userRepository, IFolderRepository folderRepository)
    : IRequestHandler<CreateFileCommand, Result<CreateFileResponse>>
{
    public async Task<Result<CreateFileResponse>> Handle(CreateFileCommand request, CancellationToken cancellationToken)
    {
        Guid folderId;
        Guid userId;
        Guid fileId = Guid.NewGuid();
        long fileSize = 0;

        try
        {
            folderId = Guid.Parse(request.FolderId);
            userId = request.UserId;
        }
        catch (Exception e)
        {
            return Result<CreateFileResponse>.Failure("Wrong id format");
        }

        var folder = await folderRepository.GetFolderByFolderId(folderId, cancellationToken);
        var user = await userRepository.GetUserByUserId(userId, cancellationToken);

        if (user == null) return Result<CreateFileResponse>.Failure("User not found");
        if (folder == null) return Result<CreateFileResponse>.Failure("Folder not found");

        try
        {
            var file = new File(fileId, request.FileName, request.MimeType, fileSize, "/", request.EncryptedKey, request.Checksum,
                folderId, userId);

            file.InitAdditionalData(folder, user);

            await fileRepository.CreateFile(file, cancellationToken);
        }
        catch (Exception e)
        {
            return Result<CreateFileResponse>.Failure(e.Message);
        }

        var response = new CreateFileResponse(fileId);

        return Result<CreateFileResponse>.Success(response);
    }
}
