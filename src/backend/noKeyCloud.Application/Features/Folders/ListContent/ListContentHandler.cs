using MediatR;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Contracts.Common;
using noKeyCloud.Contracts.Folders;

namespace noKeyCloud.Application.Features.Folders.ListContent
{
    public class ListContentHandler : IRequestHandler<ListContentQuery, Result<ListContentResponse>>
    {
        public readonly IFolderRepository _FolderRepository;
        public ListContentHandler(IFolderRepository folderRepository)
        {
            _FolderRepository = folderRepository;
        }
        public async Task<Result<ListContentResponse>> Handle(ListContentQuery request, CancellationToken cancellationToken)
        {

            var folder = await _FolderRepository.GetFilesOrContentAsync(request.FolderId, request.UserId, cancellationToken);

            if (folder.Item1 is null && folder.Item2 is null)
            {
                return Result<ListContentResponse>.Failure("Folder not found");
            }


            var response = new ListContentResponse
            (
                Files: folder.Item1.Select(f => new FileResponse
                (
                    FilesId: f.Id,
                    FileNameEncrypted: f.EncryptedName,
                    MimeType: f.MimeType,
                    SizeBytes: f.SizeBytes,
                    FileKeyEncrypted: f.EncryptedKey,
                    CheckSum: f.Checksum,
                    CreatedAt: f.CreatedAt,
                    UpdatedAt: f.UpdatedAt
                )).ToList(),

                Folders: folder.Item2.Select(f => new FolderResponse
                (
                    FolderId: f.Id,
                    NameEncrypted: f.EncryptedName,
                    FolderKeyEncrypted: f.EncryptedKey,
                    CreatedAt: f.CreatedAt,
                    UpdatedAt: f.UpdatedAt,
                    ParentFolderId: f.ParentFolderId
                )).ToList()
            );

            return Result<ListContentResponse>.Success(response);
        }
    }
}