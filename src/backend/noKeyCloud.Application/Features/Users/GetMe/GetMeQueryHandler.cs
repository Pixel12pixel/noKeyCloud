using MediatR;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Contracts.Common;
using noKeyCloud.Contracts.User;

namespace noKeyCloud.Application.Features.Users.GetMe;

public class GetMeQueryHandler(IUserRepository userRepository) : IRequestHandler<GetMeQuery, Result<GetMeResponse>>
{
    public async Task<Result<GetMeResponse>> Handle(GetMeQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserByUserId(request.UserId, cancellationToken);
        
        if (user == null)
        {
            return Result<GetMeResponse>.Failure("User not found.");
        }

        var response = new GetMeResponse(
            user.Id.ToString(),
            user.Username,
            user.Email
        );

        return Result<GetMeResponse>.Success(response);
    }
}