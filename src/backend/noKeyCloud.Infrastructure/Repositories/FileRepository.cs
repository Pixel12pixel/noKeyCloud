using Microsoft.EntityFrameworkCore;
using noKeyCloud.Application.Abstractions.Repositories;
using File = noKeyCloud.Domain.Entities.File;

namespace noKeyCloud.Infrastructure.Repositories;

public class FileRepository : IFileRepository
{
    private readonly DataContext _context;
    
    public FileRepository(DataContext context)
    {
        _context = context;
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