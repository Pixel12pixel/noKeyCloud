using MediatR;
using Microsoft.EntityFrameworkCore;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Contracts.Common;
using noKeyCloud.Contracts.File;
using File = noKeyCloud.Domain.Entities.File;

namespace noKeyCloud.Infrastructure.Repositories;



public class FileRepository : IFileRepository
{
    private const string FileExtension = ".nkc";
    private readonly DataContext _context;
    private const string BaseStoragePath = "/data";

    public FileRepository(DataContext context)
    {
        _context = context;
        
        if (!Directory.Exists(BaseStoragePath))
        {
            Directory.CreateDirectory(BaseStoragePath);
        }
    }

    public async Task CreateFile(File file, CancellationToken cancellationToken, byte[]? fileContent = null)
    {
        var fullPath = Path.Combine(BaseStoragePath, $"{file.Id}{FileExtension}");
        
        var exists = await _context.Files.AnyAsync(f => f.Id == file.Id, cancellationToken);

        try
        {
            if (exists && System.IO.File.Exists(fullPath))
            {
                if (fileContent != null)
                {
                    await System.IO.File.WriteAllBytesAsync(fullPath, fileContent, cancellationToken);
                }
            }
            else
            {
                await System.IO.File.WriteAllBytesAsync(fullPath, fileContent ?? Array.Empty<byte>(), cancellationToken);
                
                await _context.Files.AddAsync(file, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Couldn't upload content.", ex);
        }

    }

    public async Task<(Guid UserId, byte[] fileContent, Guid ParentFolderId)> GetFileById(Guid fileId, Guid userId, CancellationToken cancellationToken)
    {


        byte[] fileContent = null;
        var fullPath = Path.Combine(BaseStoragePath, $"{fileId}{FileExtension}");
        var exists = await _context.Files.AnyAsync(f => f.Id == fileId, cancellationToken);
        if (!exists)
        {
            throw new Exception("Couldn't find the file content.");
        }
        var file = await _context.Files.FirstOrDefaultAsync(f => f.Id == fileId && f.UserId == userId, cancellationToken);

        if(System.IO.File.Exists(fullPath))
        {
            fileContent = await System.IO.File.ReadAllBytesAsync(fullPath, cancellationToken);
        }

        return (file.UserId, fileContent, file.ParentFolderId);
            
    }
}