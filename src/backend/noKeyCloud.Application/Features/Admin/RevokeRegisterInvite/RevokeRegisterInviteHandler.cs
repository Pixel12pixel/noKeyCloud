using MediatR;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Contracts.Common;

namespace noKeyCloud.Application.Features.Admin.RevokeRegisterInvite;

public class RevokeRegisterInviteHandler(IRegisterInviteRepository inviteRepository) 
    : IRequestHandler<RevokeRegisterInviteCommand, Result>
{
    public async Task<Result> Handle(RevokeRegisterInviteCommand request, CancellationToken cancellationToken)
    {
        var invite = await inviteRepository.GetByIdAsync(request.Id, cancellationToken);
        if (invite == null) return Result.Failure("Invite not found.");

        await inviteRepository.DeleteAsync(invite, cancellationToken);
        return Result.Success();
    }
}