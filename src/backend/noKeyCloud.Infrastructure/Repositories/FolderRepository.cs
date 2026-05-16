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

    public async Task<(List<Domain.Entities.File>, List<Folder>)> GetFilesOrContentAsync(Guid folderId, Guid userId, CancellationToken cancellationToken)
        {
            var folder = await _context.Folders
                .Include(f => f.Files)
                .Include(f => f.SubFolders)
                .FirstOrDefaultAsync(f => f.Id == folderId && f.UserId == userId, cancellationToken);
            if (folder == null)
            {
                throw new Exception("Folder not found");
            }
            return (folder.Files.ToList(), folder.SubFolders.ToList());
        }
        
        public async Task<Folder> AddFolder(Folder folder, CancellationToken cancellationToken = default)
        {
            await _context.Folders.AddAsync(folder, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        
            return folder;
        }

}