using File = noKeyCloud.Domain.Entities.File;

namespace noKeyCloud.Application.Abstractions.Repositories;

public interface IFileRepository
{
    Task CreateFile(File file, CancellationToken cancellationToken, byte[]? fileContent = null);

    Task<(File file, byte[] content)> GetFileById(Guid fileId, Guid userId, CancellationToken cancellationToken);

    Task<byte[]> DownloadFile(File file, CancellationToken cancellationToken);
}