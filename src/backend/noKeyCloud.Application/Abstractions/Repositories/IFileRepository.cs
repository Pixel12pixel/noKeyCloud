using System.Net;
using noKeyCloud.Domain.Entities;
using File = noKeyCloud.Domain.Entities.File;

namespace noKeyCloud.Application.Abstractions.Repositories;

public interface IFileRepository
{
    
    Task CreateFile(File file, CancellationToken cancellationToken);

    Task<bool> FileExists(byte[] filename, CancellationToken cancellationToken);
}