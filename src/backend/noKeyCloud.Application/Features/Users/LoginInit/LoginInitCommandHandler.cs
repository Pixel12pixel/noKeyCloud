using MediatR;
using System.Numerics;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Application.Abstractions.Services;
using noKeyCloud.Application.Security;
using noKeyCloud.Contracts.Authenticate;
using noKeyCloud.Contracts.Common;

namespace noKeyCloud.Application.Features.Users.LoginInit;

public class LoginInitCommandHandler(IUserRepository userRepository, ISrpSessionStore sessionStore)
    : IRequestHandler<LoginInitCommand, Result<LoginInitResponse>>
{
    public async Task<Result<LoginInitResponse>> Handle(LoginInitCommand request, CancellationToken cancellationToken)
    {
        string identifier = !string.IsNullOrWhiteSpace(request.Username) 
            ? request.Username 
            : request.Email;

        var user = await userRepository.GetUserByUsernameOrEmailAsync(identifier, cancellationToken);
        if (user == null)
        {
            return Result<LoginInitResponse>.Failure("Invalid credentials.");
        }
        
        BigInteger clientA;
        try 
        {
            byte[] aBytes = Convert.FromBase64String(request.A);
            clientA = new BigInteger(aBytes, isUnsigned: true, isBigEndian: true);
        }
        catch (FormatException)
        {
            return Result<LoginInitResponse>.Failure("Invalid 'A' parameter format.");
        }
        
        var srpServer = new NativeSrpServer();
        var verifier = new BigInteger(user.Verifier, isUnsigned: true, isBigEndian: true);

        try 
        {
            var (b, B) = srpServer.GenerateChallenge(verifier);
            var S = srpServer.ComputePremasterSecret(clientA, b, verifier, B);

            var session = new SrpSession 
            {
                Username = user.Username,
                Salt = user.Salt,
                A = clientA,
                B = B,
                S = S
            };
            
            var sessionId = Guid.NewGuid();
            sessionStore.SaveSession(sessionId, user.Id, session);

            var response = new LoginInitResponse(
                Convert.ToBase64String(user.Salt),
                Convert.ToBase64String(B.ToByteArray(isUnsigned: true, isBigEndian: true)),
                sessionId
            );

            return Result<LoginInitResponse>.Success(response);
        }
        catch (Exception) 
        {
            return Result<LoginInitResponse>.Failure("Invalid client public value 'A'.");
        }
    }
}