using MediatR;
using noKeyCloud.Contracts.Common;
using noKeyCloud.Contracts.Folders;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace noKeyCloud.Application.Features.Folders.ListContent;

public record ListContentQuery(
    Guid FolderId,
    Guid UserId
    ) : IRequest<Result<ListContentResponse>>;
