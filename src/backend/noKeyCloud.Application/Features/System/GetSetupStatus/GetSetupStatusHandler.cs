using MediatR;
using noKeyCloud.Application.Abstractions.Repositories;

namespace noKeyCloud.Application.Features.System.GetSetupStatus;

public class GetSetupStatusHandler(IUserRepository userRepository) : IRequestHandler<GetSetupStatusQuery, bool>
{
    public async Task<bool> Handle(GetSetupStatusQuery request, CancellationToken cancellationToken)
    {
        bool hasUsers = await userRepository.HasAnyUsersAsync(cancellationToken);
        return !hasUsers; 
    }
}