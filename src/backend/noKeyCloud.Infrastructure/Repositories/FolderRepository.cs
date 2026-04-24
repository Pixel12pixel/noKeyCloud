using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Domain.Entities;

namespace noKeyCloud.Infrastructure.Repositories;

public class FolderRepository(DataContext context) : IFolderRepository
{
    public async Task<Folder> AddFolder(Folder folder, CancellationToken cancellationToken = default)
    {
        await context.Folders.AddAsync(folder, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        return folder;
    }
}