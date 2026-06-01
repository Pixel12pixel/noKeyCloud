using MediatR;
using noKeyCloud.Contracts.Common;
using noKeyCloud.Contracts.File;

namespace noKeyCloud.Application.Features.Files.UploadFile;

public record UploadFileCommand
(
    Guid UserId,
    byte[] FileName,
    string MimeType,
    long SizeBytes,
    byte[] EncryptedKey,
    byte[] Checksum,
    string FolderId,
    byte[] FileContent
) : IRequest<Result<UploadFileResponse>>;