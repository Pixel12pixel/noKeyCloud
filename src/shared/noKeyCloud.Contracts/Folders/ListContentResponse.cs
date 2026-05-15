using System;
using System.Collections.Generic;
using System.Text;

namespace noKeyCloud.Contracts.Folders;

public record FileResponse(
    Guid FilesId,
    byte[] FileNameEncrypted,
    string MimeType,
    long SizeBytes,
    byte[] FileKeyEncrypted,
    byte[] CheckSum,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record ListContentResponse(
    IEnumerable<FileResponse> Files, 
    IEnumerable<FolderResponse> Folders
);

public record FolderResponse(
    Guid FolderId,
    byte[] NameEncrypted,
    byte[] FolderKeyEncrypted,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    Guid? ParentFolderId
);

