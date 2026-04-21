using MediatR;
using noKeyCloud.Contracts.Authenticate;
using noKeyCloud.Contracts.Common;

namespace noKeyCloud.Application.Features.Users.LoginInit;

public record LoginInitCommand(
    string Username,
    string Email,
    string A) : IRequest<Result<LoginInitResponse>>;
