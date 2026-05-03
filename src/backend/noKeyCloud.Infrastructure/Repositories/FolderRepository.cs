using noKeyCloud.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using noKeyCloud.Application.Abstractions.Repositories;


namespace noKeyCloud.Infrastructure.Repositories
{
    public class FolderRepository : IFolderRepository
    {
        private readonly DataContext _context;
        public FolderRepository(DataContext context)
        {
            _context = context;
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
    }
    
    public async Task<Folder> AddFolder(Folder folder, CancellationToken cancellationToken = default)
    {
        await context.Folders.AddAsync(folder, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        return folder;
    }
}