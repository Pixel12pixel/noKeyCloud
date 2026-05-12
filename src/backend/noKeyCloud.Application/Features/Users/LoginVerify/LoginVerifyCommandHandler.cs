using System.Numerics;
using MediatR;
using noKeyCloud.Application.Abstractions.Services;
using noKeyCloud.Contracts.Authenticate;
using noKeyCloud.Contracts.Common;
using Org.BouncyCastle.Crypto;
using BigInteger = Org.BouncyCastle.Math.BigInteger;

namespace noKeyCloud.Application.Features.Users.LoginVerify;

public class LoginVerifyCommandHandler 
    : IRequestHandler<LoginVerifyCommand, Result<LoginVerifyResponse>>
{

    private readonly IJwtService _JwtService;
    private readonly ISrpSessionStore _sessionStore;

    public LoginVerifyCommandHandler(IJwtService jwtService, ISrpSessionStore srpSessionStore)
    {
        _JwtService = jwtService;
        _sessionStore = srpSessionStore;
    }
    public async Task<Result<LoginVerifyResponse>> Handle(LoginVerifyCommand request, CancellationToken cancellationToken)
    {
        Guid sessionIdGuid = Guid.Parse(request.SessionId);
        
        var session = _sessionStore.GetSession(sessionIdGuid);

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
        catch(CryptoException)
        {
            return Result<LoginVerifyResponse>.Failure("SRP verification failed");
        }
        
        if(!isValid) return Result<LoginVerifyResponse>.Failure("Invalid credentials");

        var serverM2 = session.CalculateServerEvidenceMessage();
        var userId = _sessionStore.GetUserId(sessionIdGuid);
        var token = await _JwtService.JwtTokenService(userId);

        if (!_sessionStore.DeleteSession(sessionIdGuid)) return Result<LoginVerifyResponse>.Failure("Could not remove session");

        var response = new LoginVerifyResponse(userId.ToString(), Convert.ToBase64String(serverM2.ToByteArrayUnsigned()), token.ToString());
        
        return Result<LoginVerifyResponse>.Success(response);
    }
}