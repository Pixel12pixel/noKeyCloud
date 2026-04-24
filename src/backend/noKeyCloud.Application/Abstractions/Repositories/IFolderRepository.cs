using noKeyCloud.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
namespace noKeyCloud.Application.Abstractions.Repositories
{
    public interface IFolderRepository
    {
        Task<(List<noKeyCloud.Domain.Entities.File>, List<Folder>)> GetFilesOrContentAsync(Guid folderId, Guid userId, CancellationToken cancellationToken);

    }
}
