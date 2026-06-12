using noKeyCloud.Domain.Entities;

namespace noKeyCloud.Application.Abstractions.Repositories;

public interface IRecoveryMethodRepository
{
    Task CreateRecoveryMethod(RecoveryMethod recoveryMethod, CancellationToken cancellationToken = default);
}