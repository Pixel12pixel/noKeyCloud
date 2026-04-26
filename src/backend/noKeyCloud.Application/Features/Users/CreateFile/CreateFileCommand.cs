using MediatR;
using noKeyCloud.Contracts.Authenticate;
using noKeyCloud.Contracts.Common;

namespace noKeyCloud.Application.Features.Users.CreateFile;

public record CreateFileCommand
(
    string UserId,
    string FileName,
    string FolderId
    ): IRequest<Result<CreateFileResponse>>;