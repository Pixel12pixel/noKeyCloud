using MediatR;
using System.Numerics;
using System.Security.Cryptography;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Application.Abstractions.Services;
using noKeyCloud.Application.Security;
using noKeyCloud.Contracts.Authenticate;
using noKeyCloud.Contracts.Common;

namespace noKeyCloud.Application.Features.Users.LoginVerify;

    public class LoginVerifyCommandHandler(
        IJwtService jwtService,
        ISrpSessionStore srpSessionStore,
        IRefreshTokenProvider refreshTokenProvider,
        IFolderRepository folderRepository)
        : IRequestHandler<LoginVerifyCommand, Result<LoginVerifyResult>>
    {

    public async Task<Result<LoginVerifyResult>> Handle(LoginVerifyCommand request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.SessionId, out Guid sessionIdGuid))
            return Result<LoginVerifyResult>.Failure("Invalid session ID format.");

        var session = srpSessionStore.GetSession(sessionIdGuid);

        if (session == null) return Result<LoginVerifyResult>.Failure("Session not found.");

        byte[] clientM1Bytes;
        try
        {
            clientM1Bytes = Convert.FromBase64String(request.M1);
        }
        catch (FormatException)
        {
            return Result<LoginVerifyResult>.Failure("Session M1 format");
        }
        
        var srpServer = new NativeSrpServer();

        try
        {
            var expectedM1 = srpServer.ComputeExpectedClientProof(session.Username, session.Salt, session.A, session.B, session.S);

            if (!CryptographicOperations.FixedTimeEquals(clientM1Bytes, expectedM1))
            {
                return Result<LoginVerifyResult>.Failure("Invalid credentials.");
            }

            var serverM2 = srpServer.ComputeServerProof(session.A, clientM1Bytes, session.S);
            var nullableUserId = srpSessionStore.GetUserId(sessionIdGuid);
            if (nullableUserId == null)
            {
                return Result<LoginVerifyResult>.Failure("Could not retrieve user associated with the session.");
            }

            var userId = nullableUserId.Value;
            var token = await jwtService.JwtTokenService(userId);

            var refreshToken = refreshTokenProvider.GenerateRefreshToken();
            await refreshTokenProvider.StoreRefreshTokenAsync(userId, refreshToken, TimeSpan.FromHours(24), cancellationToken);
            var rootFolder = await folderRepository.GetUserHomeFolder(userId, cancellationToken);

            if (!srpSessionStore.DeleteSession(sessionIdGuid)) return Result<LoginVerifyResult>.Failure("Could not remove session");
            
            var responsePayload = new LoginVerifyResponse(
                userId.ToString(), 
                Convert.ToBase64String(serverM2), 
                rootFolder.Id.ToString(),
                DateTime.UtcNow.AddMinutes(15)
            );
            
            var handlerResult = new LoginVerifyResult(
                responsePayload, 
                token, 
                refreshToken
            );

            return Result<LoginVerifyResult>.Success(handlerResult);
        }
        catch (Exception)
        {
            return Result<LoginVerifyResult>.Failure("SRP verification failed");
        }
    }
}