namespace noKeyCloud.Contracts.Folders;

public record GetAncestryResponse(
    List<FolderAncestryItem> Items);