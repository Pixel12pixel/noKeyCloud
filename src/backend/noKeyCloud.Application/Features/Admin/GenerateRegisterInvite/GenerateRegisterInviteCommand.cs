using MediatR;
using noKeyCloud.Contracts.Admin;
using noKeyCloud.Contracts.Common;

namespace noKeyCloud.Application.Features.Admin.GenerateRegisterInvite;

public record GenerateRegisterInviteCommand(int ExpirationHours = 24) : IRequest<Result<RegisterInviteResponse>>;