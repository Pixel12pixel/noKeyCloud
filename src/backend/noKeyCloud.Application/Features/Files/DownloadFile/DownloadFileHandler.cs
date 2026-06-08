using MediatR;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Contracts.Common;
using noKeyCloud.Contracts.File;

namespace noKeyCloud.Application.Features.Files.DownloadFile
{
    public class DownloadFileHandler(IFileRepository fileRepository, IMediator mediator) : IRequestHandler<DownloadFileCommand, Result<DownloadFileResponse>>
    {
        public async Task<Result<DownloadFileResponse>> Handle(DownloadFileCommand request, CancellationToken cancellationToken)
        {
            var response = await fileRepository.GetFileById(request.fileId, request.userId, cancellationToken);

            var Answer = new DownloadFileResponse
            (
                fileId: response.file.Id,
                fileContent: response.content,
                MimeType: response.file.MimeType,
                EncryptedName: response.file.EncryptedName,
                EncryptedKey: response.file.EncryptedKey,
                Checksum: response.file.Checksum
            );

            return Result<DownloadFileResponse>.Success(Answer);
        }

    }
}
