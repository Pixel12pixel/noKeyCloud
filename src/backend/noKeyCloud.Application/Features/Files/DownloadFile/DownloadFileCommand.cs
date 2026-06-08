using MediatR;
using noKeyCloud.Contracts.Common;
using noKeyCloud.Contracts.File;
using System;
using System.Collections.Generic;
using System.Text;

namespace noKeyCloud.Application.Features.Files.DownloadFile { }
public record DownloadFileCommand(
    Guid userId,
    Guid fileId

    ) : IRequest<Result<DownloadFileResponse>>;
