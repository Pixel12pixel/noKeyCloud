using MediatR;
using noKeyCloud.Contracts.Common;

namespace noKeyCloud.Application.Features.Users.RefreshSession;

public record RefreshSessionCommand(
    string RefreshToken) : IRequest<Result<RefreshSessionResult>>;