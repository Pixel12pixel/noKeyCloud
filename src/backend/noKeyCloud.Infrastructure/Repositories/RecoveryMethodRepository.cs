using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Domain.Entities;

namespace noKeyCloud.Infrastructure.Repositories;

public class RecoveryMethodRepository : IRecoveryMethodRepository
{
    private readonly DataContext _context;

    public RecoveryMethodRepository(DataContext context)
    {
        _context = context;
    }
    
    public async Task CreateRecoveryMethod(RecoveryMethod recoveryMethod, CancellationToken cancellationToken = default)
    {
        await _context.RecoveryMethods.AddAsync(recoveryMethod, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
    
}