using MediatR;
using noKeyCloud.Contracts.Admin;

namespace noKeyCloud.Application.Features.Admin.GetActiveRegisterInvites;

public record GetActiveRegisterInvitesQuery : IRequest<IEnumerable<RegisterInviteResponse>>;