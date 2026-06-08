using MediatR;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Contracts.Admin;

namespace noKeyCloud.Application.Features.Admin.GetActiveRegisterInvites;

public class GetActiveRegisterInvitesHandler(IRegisterInviteRepository inviteRepository) 
    : IRequestHandler<GetActiveRegisterInvitesQuery, IEnumerable<RegisterInviteResponse>>
{
    public async Task<IEnumerable<RegisterInviteResponse>> Handle(GetActiveRegisterInvitesQuery request, CancellationToken cancellationToken)
    {
        var invites = await inviteRepository.GetActiveInvitesAsync(cancellationToken);
        return invites.Select(i => new RegisterInviteResponse(i.Id, i.Code, i.CreatedAt, i.ExpiresAt));
    }
}