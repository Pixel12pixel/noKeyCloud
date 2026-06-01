using MediatR;
using noKeyCloud.Contracts.Common;
using noKeyCloud.Contracts.User;

namespace noKeyCloud.Application.Features.Users.GetMe;

public record GetMeQuery(Guid UserId) : IRequest<Result<GetMeResponse>>;