using System.Security.Cryptography;
using MediatR;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Contracts.Admin;
using noKeyCloud.Contracts.Common;
using noKeyCloud.Domain.Entities;

namespace noKeyCloud.Application.Features.Admin.GenerateRegisterInvite;

public class GenerateRegisterInviteHandler(IRegisterInviteRepository inviteRepository) 
    : IRequestHandler<GenerateRegisterInviteCommand, Result<RegisterInviteResponse>>
{
    public async Task<Result<RegisterInviteResponse>> Handle(GenerateRegisterInviteCommand request, CancellationToken cancellationToken)
    {
        var randomBytes = new byte[12];
        RandomNumberGenerator.Fill(randomBytes);
        var code = Convert.ToBase64String(randomBytes)
            .Replace("+", "").Replace("/", "").TrimEnd('='); 
        
        var expiresAt = DateTime.UtcNow.AddHours(request.ExpirationHours);
        
        var invite = new RegisterInvite(
            Guid.NewGuid(), 
            code, 
            DateTime.UtcNow, 
            expiresAt
        );

        await inviteRepository.AddAsync(invite, cancellationToken);
        
        return Result<RegisterInviteResponse>.Success(new RegisterInviteResponse(invite.Id, invite.Code, invite.CreatedAt, invite.ExpiresAt));
    }
}
