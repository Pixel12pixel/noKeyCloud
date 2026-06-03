using noKeyCloud.Domain.Entities;

namespace noKeyCloud.Application.Abstractions.Repositories;

public interface IRegisterInviteRepository
{
    Task AddAsync(RegisterInvite invite, CancellationToken cancellationToken = default);
    Task<RegisterInvite?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task UpdateAsync(RegisterInvite invite, CancellationToken cancellationToken = default);
}