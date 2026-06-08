using MediatR;
using noKeyCloud.Contracts.Common;

namespace noKeyCloud.Application.Features.Admin.RevokeRegisterInvite;

public record RevokeRegisterInviteCommand(Guid Id) : IRequest<Result>;