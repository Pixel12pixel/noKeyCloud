using MediatR;
using noKeyCloud.Contracts.Common;
using noKeyCloud.Contracts.Folders;

namespace noKeyCloud.Application.Features.Folders.GetAncestry;

public record GetAncestryQuery(
    Guid UserId,
    Guid FolderId) : IRequest<Result<GetAncestryResponse>>;
