using Microsoft.EntityFrameworkCore;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Domain.Entities;

namespace noKeyCloud.Infrastructure.Repositories;

public class RegisterInviteRepository(DataContext context) : IRegisterInviteRepository
{
    public async Task AddAsync(RegisterInvite invite, CancellationToken cancellationToken = default)
    {
        await context.RegisterInvites.AddAsync(invite, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<RegisterInvite?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await context.RegisterInvites
            .SingleOrDefaultAsync(i => i.Code == code, cancellationToken);
    }

    public async Task UpdateAsync(RegisterInvite invite, CancellationToken cancellationToken = default)
    {
        context.RegisterInvites.Update(invite);
        await context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task<IEnumerable<RegisterInvite>> GetActiveInvitesAsync(CancellationToken cancellationToken = default)
    {
        return await context.RegisterInvites
            .Where(i => !i.IsUsed && (i.ExpiresAt == null || i.ExpiresAt > DateTime.UtcNow))
            .ToListAsync(cancellationToken);
    }

    public async Task<RegisterInvite?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.RegisterInvites.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task DeleteAsync(RegisterInvite invite, CancellationToken cancellationToken = default)
    {
        context.RegisterInvites.Remove(invite);
        await context.SaveChangesAsync(cancellationToken);
    }
}