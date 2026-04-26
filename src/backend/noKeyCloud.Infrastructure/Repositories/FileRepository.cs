using Microsoft.EntityFrameworkCore;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Domain.Entities;
using File = noKeyCloud.Domain.Entities.File;

namespace noKeyCloud.Infrastructure.Repositories;

public class FileRepository :  IFileRepository
{
    private readonly DataContext _context;
    
    public FileRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserByUserId(Guid userId, CancellationToken cancellationToken)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId,
            cancellationToken);
    }

    public async Task<Folder?> GetFolderByFolderId(Guid folderId, CancellationToken cancellationToken)
    {
        return await _context.Folders.FirstOrDefaultAsync(u => u.Id == folderId,
            cancellationToken);
    }

    public async Task CreateFile(File file,  CancellationToken cancellationToken)
    {
        await _context.Files.AddAsync(file, cancellationToken);
    }
    
    public async Task<bool> FileExists(byte[] fileName, CancellationToken cancellationToken)
    {
        return await _context.Files
            .AnyAsync(f => f.EncryptedName == fileName, cancellationToken);
    }
}