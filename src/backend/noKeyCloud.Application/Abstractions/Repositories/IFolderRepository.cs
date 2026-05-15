using noKeyCloud.Domain.Entities;
using File = System.IO.File;
using System;
using System.Collections.Generic;
using System.Text;

namespace noKeyCloud.Application.Abstractions.Repositories;

public interface IFolderRepository
{
    
    Task<Folder?> GetFolderByFolderId(Guid folderId, CancellationToken cancellationToken);
    
    Task<(List<noKeyCloud.Domain.Entities.File>, List<Folder>)> GetFilesOrContentAsync(Guid folderId, Guid userId, CancellationToken cancellationToken);

    Task<Folder> AddFolder(Folder folder, CancellationToken cancellationToken = default);
}