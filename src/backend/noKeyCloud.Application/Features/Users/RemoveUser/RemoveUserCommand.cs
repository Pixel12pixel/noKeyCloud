using MediatR;
using noKeyCloud.Contracts.Authenticate;
using noKeyCloud.Contracts.Common;

namespace noKeyCloud.Application.Features.Users.RemoveUser;

public record RemoveUserCommand(
    Guid Id) : IRequest<Result<RemoveUserResponse>>;