using MediatR;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Application.Abstractions.Services;
using noKeyCloud.Contracts.Authenticate;
using noKeyCloud.Contracts.Common;
using Org.BouncyCastle.Crypto;
using BigInteger = Org.BouncyCastle.Math.BigInteger;

namespace noKeyCloud.Application.Features.Users.LoginVerify;

public class LoginVerifyCommandHandler(
    IJwtService jwtService,
    ISrpSessionStore srpSessionStore,
    IRefreshTokenProvider refreshTokenProvider,
    IFolderRepository folderRepository)
    : IRequestHandler<LoginVerifyCommand, Result<LoginVerifyResponse>>
{

    public async Task<Result<LoginVerifyResponse>> Handle(LoginVerifyCommand request, CancellationToken cancellationToken)
    {
        Guid sessionIdGuid = Guid.Parse(request.SessionId);

        var session = srpSessionStore.GetSession(sessionIdGuid);

        if (session == null) return Result<LoginVerifyResponse>.Failure("Session not found.");

        BigInteger clientM1;

        try
        {
            byte[] M1Byte = Convert.FromBase64String(request.M1);

            clientM1 = new BigInteger(1, M1Byte);
        }
        catch (FormatException)
        {
            return Result<LoginVerifyResponse>.Failure("Session M1 format");
        }

        bool isValid;
        try
        {
            isValid = session.VerifyClientEvidenceMessage(clientM1);
        }
        catch (CryptoException)
        {
            return Result<LoginVerifyResponse>.Failure("SRP verification failed");
        }

        if (!isValid) return Result<LoginVerifyResponse>.Failure("Invalid credentials.");

        var serverM2 = session.CalculateServerEvidenceMessage();
        var nullableUserId = srpSessionStore.GetUserId(sessionIdGuid);
        if (nullableUserId == null)
        {
            return Result<LoginVerifyResponse>.Failure("Could not retrieve user associated with the session.");
        }


        var userId = nullableUserId.Value;
        var token = await jwtService.JwtTokenService(userId);

        var refreshToken = refreshTokenProvider.GenerateRefreshToken();
        await refreshTokenProvider.StoreRefreshTokenAsync(userId, refreshToken, TimeSpan.FromHours(24), cancellationToken);
        var RootFolder = await folderRepository.GetUserHomeFolder(userId, cancellationToken);

        if (!srpSessionStore.DeleteSession(sessionIdGuid)) return Result<LoginVerifyResponse>.Failure("Could not remove session");

        var response = new LoginVerifyResponse(userId.ToString(), Convert.ToBase64String(serverM2.ToByteArrayUnsigned()), token.ToString(), refreshToken, RootFolder.Id.ToString());

        return Result<LoginVerifyResponse>.Success(response);
    }
}