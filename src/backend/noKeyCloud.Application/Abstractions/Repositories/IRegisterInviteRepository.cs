using noKeyCloud.Domain.Entities;

namespace noKeyCloud.Application.Abstractions.Repositories;

public interface IRegisterInviteRepository
{
    Task AddAsync(RegisterInvite invite, CancellationToken cancellationToken = default);
    Task<RegisterInvite?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task UpdateAsync(RegisterInvite invite, CancellationToken cancellationToken = default);
    Task<IEnumerable<RegisterInvite>> GetActiveInvitesAsync(CancellationToken cancellationToken = default);
    Task<RegisterInvite?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task DeleteAsync(RegisterInvite invite, CancellationToken cancellationToken = default);
}