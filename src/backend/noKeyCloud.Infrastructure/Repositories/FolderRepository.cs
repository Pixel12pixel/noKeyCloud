using Microsoft.EntityFrameworkCore;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Domain.Entities;

namespace noKeyCloud.Infrastructure.Repositories;

public class FolderRepository : IFolderRepository
{
    private readonly DataContext _context;
    
    public FolderRepository(DataContext context)
    {
        _context = context;
    }
    
    public async Task<Folder?> GetFolderByFolderId(Guid folderId, CancellationToken cancellationToken)
    {
        return await _context.Folders.FirstOrDefaultAsync(u => u.Id == folderId,
            cancellationToken);
    }
}