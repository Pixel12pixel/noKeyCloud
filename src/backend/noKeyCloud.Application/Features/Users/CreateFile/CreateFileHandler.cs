using System.Text;
using MediatR;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Contracts.Authenticate;
using noKeyCloud.Contracts.Common;
using File = noKeyCloud.Domain.Entities.File;

namespace noKeyCloud.Application.Features.Users.CreateFile;

public class CreateFileHandler(IFileRepository fileRepository)
    : IRequestHandler<CreateFileCommand, Result<CreateFileResponse>>
{
    public async Task<Result<CreateFileResponse>> Handle(CreateFileCommand request, CancellationToken cancellationToken)
    {
        Guid folderId;
        Guid userId;
        Guid fileId =  Guid.NewGuid();
        long fileSize = 0;
        byte[] encryptedName = Encoding.UTF8.GetBytes(request.FileName);

        try
        {
            folderId = Guid.Parse(request.FolderId);
            userId = Guid.Parse(request.UserId);
        }
        catch(Exception e)
        {
            return Result<CreateFileResponse>.Failure("Wrong id format");
        }
        
        var folder = await fileRepository.GetFolderByFolderId(folderId, cancellationToken);
        var user = await fileRepository.GetUserByUserId(userId, cancellationToken);
        
        if (user == null) return Result<CreateFileResponse>.Failure("User not found");
        if (folder == null) return Result<CreateFileResponse>.Failure("Folder not found");
        
        if (await fileRepository.FileExists(encryptedName, cancellationToken))
        {
            return Result<CreateFileResponse>.Failure("File already exists");
        }
        
        try
        {
            var file = new File(fileId, encryptedName, string.Empty, fileSize, "/", [], [],
                folderId, userId, folder, user);
            
            await fileRepository.CreateFile(file,  cancellationToken);
        }
        catch (Exception e)
        {
            return Result<CreateFileResponse>.Failure(e.Message);
        }
        
        var response = new CreateFileResponse(
            request.FileName,
            fileId,
            fileSize
        );

        return Result<CreateFileResponse>.Success(response);
    }
}
