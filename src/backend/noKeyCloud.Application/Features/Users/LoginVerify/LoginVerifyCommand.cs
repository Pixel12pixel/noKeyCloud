using MediatR;
using noKeyCloud.Contracts.Authenticate;
using noKeyCloud.Contracts.Common;

namespace noKeyCloud.Application.Features.Users.LoginVerify;

public record LoginVerifyCommand(
    string SessionId,
    string M1) : IRequest<Result<LoginVerifyResponse>>;