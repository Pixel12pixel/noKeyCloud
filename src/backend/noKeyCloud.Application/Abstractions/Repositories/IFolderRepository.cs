using noKeyCloud.Domain.Entities;
using File = System.IO.File;

namespace noKeyCloud.Application.Abstractions.Repositories;

public interface IFolderRepository
{
    
    Task<Folder?> GetFolderByFolderId(Guid folderId, CancellationToken cancellationToken);
    
}