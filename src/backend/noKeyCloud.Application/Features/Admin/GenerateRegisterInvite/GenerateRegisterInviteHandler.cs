using System.Security.Cryptography;
using MediatR;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Contracts.Common;
using noKeyCloud.Domain.Entities;

namespace noKeyCloud.Application.Features.Admin.GenerateRegisterInvite;

public class GenerateRegisterInviteHandler(IRegisterInviteRepository inviteRepository) 
    : IRequestHandler<GenerateRegisterInviteCommand, Result<string>>
{
    public async Task<Result<string>> Handle(GenerateRegisterInviteCommand request, CancellationToken cancellationToken)
    {
        var randomBytes = new byte[12];
        RandomNumberGenerator.Fill(randomBytes);
        var code = Convert.ToBase64String(randomBytes)
            .Replace("+", "").Replace("/", "").TrimEnd('='); 

        var invite = new RegisterInvite(
            Guid.NewGuid(), 
            code, 
            DateTime.UtcNow, 
            DateTime.UtcNow.AddDays(request.ExpirationDays)
        );

        await inviteRepository.AddAsync(invite, cancellationToken);
        
        return Result<string>.Success(code);
    }
}
