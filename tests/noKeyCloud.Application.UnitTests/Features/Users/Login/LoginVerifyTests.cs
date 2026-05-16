using System.Text;
using Moq;
using noKeyCloud.Application.Abstractions.Services;
using noKeyCloud.Application.Features.Users.LoginVerify;
using Org.BouncyCastle.Crypto.Agreement.Srp;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Security;
using BigInteger = Org.BouncyCastle.Math.BigInteger;

namespace noKeyCloud_apiUnitTests.Features.Users.Login;

public class LoginVerifyTests
{
    private readonly Mock<ISrpSessionStore> _sessionStoreMock;
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly LoginVerifyCommandHandler _handler;
    private readonly Mock<IRefreshTokenProvider> _refreshTokenProviderMock;

    public LoginVerifyTests()
    {
        _sessionStoreMock = new Mock<ISrpSessionStore>();

        _refreshTokenProviderMock = new Mock<IRefreshTokenProvider>();
        
        _refreshTokenProviderMock.Setup(x => x.GenerateRefreshToken())
            .Returns("dummy-refresh-token");

        _jwtServiceMock = new Mock<IJwtService>();
        _jwtServiceMock
            .Setup(x => x.JwtTokenService(It.IsAny<Guid?>()))
            .ReturnsAsync("fake-jwt-token");
        
        _handler = new LoginVerifyCommandHandler(_jwtServiceMock.Object, _sessionStoreMock.Object, _refreshTokenProviderMock.Object);

    }

    [Fact]
    public async Task Handle_WhenSessionNotFound_ShouldReturnFailure()
    {
        var sessionId = Guid.NewGuid();

        _sessionStoreMock
            .Setup(x => x.GetSession(sessionId))
            .Returns((Srp6Server?)null);

        var command = new LoginVerifyCommand(sessionId.ToString(), "1234567890");
        
        var result = await _handler.Handle(command, CancellationToken.None);
        
        Assert.False(result.IsSuccess);
        Assert.Equal("Session not found.", result.Error);
    }
    
    [Fact]
    public async Task Handle_WhenM1IsValid_ShouldReturnSuccess()
    {
        BigInteger N = new BigInteger("32317006071311007300714876688669951960444102669715484032130345427524655138867890893197201411522913463688717960921898019494119559150490921095088152386448283120630877367300996091750197750389652106796057638384067568276792218642619756161838094338476170470581645852036305042887575891541065808607552399123930385521914333389668342420684974786564569494856176035326322058077805659331026192708460314150258592864177116725943603718461857357598351152334063994785580370721665417662212881203104945914551140008147396357886767669820042828793708588252247031092071155540224751031064253209884099238184688246467489498721336450133889385773");
        BigInteger G = BigInteger.ValueOf(2);

        var digest = new Sha256Digest();
        var random = new SecureRandom();

        var salt = new byte[] { 1, 2, 3 };
        var username = "user";
        var password = "password";
        
        var identityBytes = Encoding.UTF8.GetBytes(username);
        var passwordBytes = Encoding.UTF8.GetBytes(password);

        var gen = new Srp6VerifierGenerator();
        gen.Init(N, G, digest);
        var v = gen.GenerateVerifier(salt, identityBytes, passwordBytes);
        
        var server = new Srp6Server();
        server.Init(N, G, v, digest, random);
        
        var client = new Srp6Client();
        client.Init(N, G, digest, random);

        var A = client.GenerateClientCredentials(salt, identityBytes, passwordBytes);
        var B = server.GenerateServerCredentials();

        client.CalculateSecret(B);
        server.CalculateSecret(A);

        var M1 = client.CalculateClientEvidenceMessage();

        var sessionId = Guid.NewGuid();
        var testUserId = Guid.NewGuid();

        _sessionStoreMock
            .Setup(x => x.GetSession(sessionId))
            .Returns(server);
        
        _sessionStoreMock
            .Setup(x => x.GetUserId(sessionId))
            .Returns(testUserId);

        _sessionStoreMock
            .Setup(x => x.DeleteSession(sessionId))
            .Returns(true);

        var command = new LoginVerifyCommand(
            sessionId.ToString(),
            Convert.ToBase64String(M1.ToByteArrayUnsigned())
        );

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        
        Assert.Equal("dummy-refresh-token", result.Value.RefreshToken);
        
        _refreshTokenProviderMock.Verify(x => x.StoreRefreshTokenAsync(
            It.IsAny<Guid>(),
            "dummy-refresh-token",
            It.IsAny<TimeSpan>(),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }
    
    [Fact]
    public async Task Handle_WhenM1IsInvalid_ShouldReturnFailure()
    {
        var sessionId = Guid.NewGuid();

        BigInteger N = new BigInteger("32317006071311007300714876688669951960444102669715484032130345427524655138867890893197201411522913463688717960921898019494119559150490921095088152386448283120630877367300996091750197750389652106796057638384067568276792218642619756161838094338476170470581645852036305042887575891541065808607552399123930385521914333389668342420684974786564569494856176035326322058077805659331026192708460314150258592864177116725943603718461857357598351152334063994785580370721665417662212881203104945914551140008147396357886767669820042828793708588252247031092071155540224751031064253209884099238184688246467489498721336450133889385773");
        BigInteger G = BigInteger.ValueOf(2);

        var fakeVerifier = new byte[256];
        fakeVerifier[0] = 1;
        var verifier = new BigInteger(1, fakeVerifier);

        var A = new BigInteger(1, new byte[] { 2, 4, 6, 8 });

        var server = new Srp6Server();
        server.Init(N, G, verifier, new Sha256Digest(), new SecureRandom());

        server.GenerateServerCredentials();
        server.CalculateSecret(A);

        _sessionStoreMock
            .Setup(x => x.GetSession(sessionId))
            .Returns(server);

        var fakeM1 = Convert.ToBase64String(new byte[] { 9, 9, 9 });
        
        var command = new LoginVerifyCommand(sessionId.ToString(), fakeM1);
        
        var result = await _handler.Handle(command, CancellationToken.None);
        
        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid credentials.", result.Error);
    }
}