using MediatR;
using noKeyCloud.Contracts.Common;
using noKeyCloud.Contracts.File;

namespace noKeyCloud.Application.Features.Files.CreateFile;

public record CreateFileCommand
(
    Guid UserId,
    string FileName,
    string FolderId
    ) : IRequest<Result<CreateFileResponse>>;