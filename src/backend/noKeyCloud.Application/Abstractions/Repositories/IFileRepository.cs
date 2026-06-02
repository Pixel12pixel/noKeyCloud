using File = noKeyCloud.Domain.Entities.File;

namespace noKeyCloud.Application.Abstractions.Repositories;

public interface IFileRepository
{
    Task CreateFile(File file, CancellationToken cancellationToken, byte[]? fileContent = null);

    Task<(Guid UserId, byte[] fileContent, Guid ParentFolderId)> GetFileById(Guid fileId, Guid userId, CancellationToken cancellationToken);
}