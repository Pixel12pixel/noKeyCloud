using MediatR;
using noKeyCloud.Contracts.File;
using noKeyCloud.Contracts.Common;

namespace noKeyCloud.Application.Features.Files.CreateFile;

public record CreateFileCommand
(
    string UserId,
    string FileName,
    string FolderId
    ): IRequest<Result<CreateFileResponse>>;