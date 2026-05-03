using MediatR;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Contracts.Common;
using noKeyCloud.Contracts.Folders;
using System;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Text;

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

            var response = new ListContentResponse
            (
                Files: folder.Item1.Select(f => new FileResponse
                (
                    FilesId: f.Id,
                    FileNameEncrypted: f.EncryptedName,
                    MimeType: f.MimeType,
                    SizeBytes: f.SizeBytes,
                    FileKeyEncrypted: f.EncryptedKey,
                    CheckSum : f.Checksum,
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