using MediatR;
using noKeyCloud.Contracts.Common;
using noKeyCloud.Contracts.Folders;

namespace noKeyCloud.Application.Features.Folders.ListContent;

public record ListContentQuery(
    Guid FolderId,
    Guid UserId
    ) : IRequest<Result<ListContentResponse>>;



