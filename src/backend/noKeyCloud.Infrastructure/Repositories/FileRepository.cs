using Microsoft.EntityFrameworkCore;
using noKeyCloud.Application.Abstractions.Repositories;
using File = noKeyCloud.Domain.Entities.File;

namespace noKeyCloud.Infrastructure.Repositories;



public class FileRepository : IFileRepository
{
    private const string FileExtension = ".nkc";
    private readonly DataContext _context;

    public FileRepository(DataContext context)
    {
        _context = context;
    }

    public async Task CreateFile(File file, CancellationToken cancellationToken, byte[]? fileContent = null)
    {
        string storagePath = "";
        var filename = file.Id;

        if (Directory.Exists(storagePath) is true
            && System.IO.File.Exists(filename + FileExtension))
        {
            try
            {
                using FileStream fs = System.IO.File.Create(filename + FileExtension);
                System.IO.File.WriteAllBytes(filename + FileExtension, fileContent);
            }
            catch
            {
                throw new Exception("Couldnt Upload Content");
            }
        }
        else
        {
            using FileStream fs = System.IO.File.Create(filename + FileExtension);
            await _context.Files.AddAsync(file, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

    }
}