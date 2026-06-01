using MediatR;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Contracts.Common;
using noKeyCloud.Contracts.File;
using System.Text;
using File = noKeyCloud.Domain.Entities.File;

namespace noKeyCloud.Application.Features.Files.UploadFile;

public class UploadFileHandler(IFileRepository fileRepository, IUserRepository userRepository, IFolderRepository folderRepository)
    : IRequestHandler<UploadFileCommand, Result<UploadFileResponse>>
{
    public async Task<Result<UploadFileResponse>> Handle(UploadFileCommand request, CancellationToken cancellationToken)
    {
        Guid folderId;
        Guid userId;
        Guid fileId = Guid.NewGuid();
        byte[] encryptedName = request.FileName;

        try
        {
            folderId = Guid.Parse(request.FolderId);
            userId = request.UserId;
        }
        catch (Exception e)
        {
            return Result<UploadFileResponse>.Failure("Wrong id format");
        }

        var folder = await folderRepository.GetFolderByFolderId(folderId, cancellationToken);
        var user = await userRepository.GetUserByUserId(userId, cancellationToken);

        if (user == null) return Result<UploadFileResponse>.Failure("User not found");
        if (folder == null) return Result<UploadFileResponse>.Failure("Folder not found");

        try
        {
            var file = new File(fileId, request.FileName, request.MimeType, request.SizeBytes, "/", request.EncryptedKey, request.Checksum,
                folderId, userId);

            file.InitAdditionalData(folder, user);

            await fileRepository.CreateFile(file, cancellationToken, request.FileContent);
        }
        catch (Exception e)
        {
            return Result<UploadFileResponse>.Failure(e.Message);
        }

        var response = new UploadFileResponse(fileId);

        return Result<UploadFileResponse>.Success(response);
    }
}
