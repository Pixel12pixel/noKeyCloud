using MediatR;
using noKeyCloud.Contracts.Common;

namespace noKeyCloud.Application.Features.Admin.GenerateRegisterInvite;

public record GenerateRegisterInviteCommand(int ExpirationDays = 7) : IRequest<Result<string>>;