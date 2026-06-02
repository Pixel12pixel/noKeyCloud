using noKeyCloud.Contracts.File;
using File = noKeyCloud.Domain.Entities.File;

namespace noKeyCloud.Application.Abstractions.Repositories;

public interface IFileRepository
{
    Task CreateFile(File file, CancellationToken cancellationToken, byte[]? fileContent = null);

    Task<DownloadFileResponse> GetFileById(Guid fileId, Guid userId, CancellationToken cancellationToken);

    Task<byte[]> DownloadFile(File file, CancellationToken cancellationToken);
}