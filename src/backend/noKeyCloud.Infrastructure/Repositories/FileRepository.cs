using Microsoft.EntityFrameworkCore;
using noKeyCloud.Application.Abstractions.Repositories;
using File = noKeyCloud.Domain.Entities.File;

namespace noKeyCloud.Infrastructure.Repositories;



public class FileRepository : IFileRepository
{
    private const string FileExtension = ".nkc";
    private readonly DataContext _context;
    private readonly string _baseStoragePath;

    public FileRepository(DataContext context)
    {
        _context = context;
        
        var envPath = Environment.GetEnvironmentVariable("BASE_UPLOAD_STORAGE_PATH");
        if (string.IsNullOrWhiteSpace(envPath))
        {
            throw new InvalidOperationException("Required environment variable 'BASE_UPLOAD_STORAGE_PATH' is missing or empty. Configure 'BASE_UPLOAD_STORAGE_PATH' before starting the application.");
        }
        _baseStoragePath = envPath;

        if (!Directory.Exists(_baseStoragePath))
        {
            Directory.CreateDirectory(_baseStoragePath);
        }
    }

    public async Task CreateFile(File file, CancellationToken cancellationToken, byte[]? fileContent = null)
    {
        var fullPath = Path.Combine(_baseStoragePath, $"{file.Id}{FileExtension}");

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

    public async Task<(File, byte[])> GetFileById(Guid fileId, Guid userId, CancellationToken cancellationToken)
    {
        var file = await _context.Files.FirstOrDefaultAsync(f => f.Id == fileId, cancellationToken);
        if (file == null)
        {
            throw new Exception("Didnt find the file");
        }

        if (file.UserId == userId)
        {
            var fileContent = await DownloadFile(file, cancellationToken);
            return (file, fileContent);
        }
        else
        {
            throw new Exception("Unauthorized access to the file.");
        }


    }

    public async Task<byte[]> DownloadFile(File file, CancellationToken cancellationToken)
    {
        var fullPath = Path.Combine(_baseStoragePath, $"{file.Id}{FileExtension}");
        if (!System.IO.File.Exists(fullPath))
        {
            throw new Exception("Couldn't find the file content.");
        }
        var fileContent = await System.IO.File.ReadAllBytesAsync(fullPath, cancellationToken);
        return fileContent;
    }


}