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

            return Result<DownloadFileResponse>.Success(response);
        }

    }
}
