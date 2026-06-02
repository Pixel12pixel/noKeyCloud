using MediatR;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Contracts.Common;
using noKeyCloud.Contracts.File;
using System;
using System.Collections.Generic;
using System.Text;

namespace noKeyCloud.Application.Features.Files.DownloadFile
{
    public class DownloadFileHandler(IFileRepository fileRepository, IMediator mediator) : IRequestHandler<DownloadFileCommand, Result<DownloadFileResponse>>
    {

        public async Task<Result<DownloadFileResponse>> Handle(DownloadFileCommand request, CancellationToken cancellationToken)
        {
            var file = await fileRepository.GetFileById(request.fileId, request.userId, cancellationToken);
            

            var response = new DownloadFileResponse(file.UserId, file.fileContent, file.ParentFolderId);
            return Result<DownloadFileResponse>.Success(response);
        }


    }
}
