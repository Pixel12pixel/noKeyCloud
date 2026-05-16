using System.ComponentModel.DataAnnotations;

namespace noKeyCloud.api.Controllers.DTOs;

public record CreateFolderRequest(
    Guid UserId, // TODO : Temporarily provided via API request body later extract the UserId from the JWT 
    string Name,
    Guid? ParentFolderId);