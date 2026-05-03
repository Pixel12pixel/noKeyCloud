using System;
using System.Collections.Generic;
using System.Text;
using noKeyCloud.Domain.Entities;

namespace noKeyCloud.Application.Abstractions.Repositories
{
    public interface IFolderRepository
    {
        Task<(List<noKeyCloud.Domain.Entities.File>, List<Folder>)> GetFilesOrContentAsync(Guid folderId, Guid userId, CancellationToken cancellationToken);

        Task<Folder> AddFolder(Folder folder, CancellationToken cancellationToken = default);
    }
}