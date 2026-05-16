using MediatR;
using Org.BouncyCastle.Crypto.Agreement.Srp;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using noKeyCloud.Application.Abstractions.Repositories;
using noKeyCloud.Application.Abstractions.Services;
using noKeyCloud.Contracts.Authenticate;
using noKeyCloud.Contracts.Common;

namespace noKeyCloud.Application.Features.Users.LoginInit;

public class LoginInitCommandHandler(IUserRepository userRepository, ISrpSessionStore sessionStore)
    : IRequestHandler<LoginInitCommand, Result<LoginInitResponse>>
{
    // 2048-bit prime number
    private static readonly BigInteger N = new BigInteger("32317006071311007300714876688669951960444102669715484032130345427524655138867890893197201411522913463688717960921898019494119559150490921095088152386448283120630877367300996091750197750389652106796057638384067568276792218642619756161838094338476170470581645852036305042887575891541065808607552399123930385521914333389668342420684974786564569494856176035326322058077805659331026192708460314150258592864177116725943603718461857357598351152334063994785580370721665417662212881203104945914551140008147396357886767669820042828793708588252247031092071155540224751031064253209884099238184688246467489498721336450133889385773");
    private static readonly BigInteger G = BigInteger.ValueOf(2);

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
            clientA = new BigInteger(1, aBytes);
        }
        catch (FormatException)
        {
            return Result<LoginInitResponse>.Failure("Invalid 'A' parameter format.");
        }
        
        if (clientA.Mod(N).Equals(BigInteger.Zero))
        {
            return Result<LoginInitResponse>.Failure("Invalid client public value 'A' (A % N == 0).");
        }

        var verifier = new BigInteger(1, user.Verifier);
        
        var srpServer = new Srp6Server();
        srpServer.Init(N, G, verifier, new Sha256Digest(), new SecureRandom());

        var serverB = srpServer.GenerateServerCredentials();
        
        try 
        {
            srpServer.CalculateSecret(clientA);
        }
        catch (CryptoException) 
        {
            return Result<LoginInitResponse>.Failure("Invalid client public value 'A'.");
        }
        
        var sessionId = Guid.NewGuid();
        sessionStore.SaveSession(sessionId, user.Id, srpServer);

        var response = new LoginInitResponse(
            Convert.ToBase64String(user.Salt),
            Convert.ToBase64String(serverB.ToByteArrayUnsigned()),
            sessionId
        );

        return Result<LoginInitResponse>.Success(response);
    }
}