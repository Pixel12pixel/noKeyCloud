using System.Net;
using noKeyCloud.Domain.Entities;
using File = noKeyCloud.Domain.Entities.File;

namespace noKeyCloud.Application.Abstractions.Repositories;

public interface IFileRepository
{
    Task<User?> GetUserByUserId(Guid userId, CancellationToken cancellationToken);
    
    Task<Folder?> GetFolderByFolderId(Guid folderId, CancellationToken cancellationToken);
    
    Task CreateFile(File file, CancellationToken cancellationToken);

    Task<bool> FileExists(byte[] filename, CancellationToken cancellationToken);
}