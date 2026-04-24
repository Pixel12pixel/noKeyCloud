using noKeyCloud.Domain.Entities;

namespace noKeyCloud.Application.Abstractions.Repositories;

public interface IFolderRepository
{
    Task<Folder> AddFolder(Folder folder, CancellationToken cancellationToken = default);
}