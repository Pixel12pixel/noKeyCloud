using MediatR;
using noKeyCloud.Contracts.Common;
using noKeyCloud.Contracts.File;

namespace noKeyCloud.Application.Features.Files.CreateFile;

public record CreateFileCommand
(
    Guid UserId,
    byte[] FileName,
    string MimeType,
    byte[] EncryptedKey,
    byte[] Checksum,
    string FolderId
    ) : IRequest<Result<CreateFileResponse>>;