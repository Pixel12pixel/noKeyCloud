using MediatR;
using noKeyCloud.Contracts.Authenticate;
using noKeyCloud.Contracts.Common;

namespace noKeyCloud.Application.Features.Users.RefreshSession;

public record RefreshSessionCommand(
    Guid UserId,
    string RefreshToken) : IRequest<Result<RefreshSessionResponse>>;