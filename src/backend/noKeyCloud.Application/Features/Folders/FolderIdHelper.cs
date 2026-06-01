using System.Security.Cryptography;
using System.Text;

namespace noKeyCloud.Application.Features.Folders;

public static class FolderIdHelper
{
    /// Generates a unique root folder ID based on the UserId.
    public static Guid GenerateRootFolderId(Guid userId)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes($"{userId}:root"));
        var guidBytes = new byte[16];
        Array.Copy(hash, guidBytes, guidBytes.Length);
        return new Guid(guidBytes);
    }
}