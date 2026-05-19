namespace noKeyCloud.api.Controllers.DTOs;

public record CreateFolderRequest(
    string Name,
    Guid? ParentFolderId);