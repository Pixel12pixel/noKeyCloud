using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace noKeyCloud.Application.Security;

public class NativeSrpServer
{
    // N = 2048 bit prime number from RFC 5054
    private static readonly string N_hex =
        "AC6BDB41324A9A9BF166DE5E1389582FAF72B6651987EE07FC3192943DB56050A37329CBB4A099ED8193E0757767A13DD52312AB4B03310DCD7F48A9DA04FD50E8083969EDB767B0CF6095179A163AB3661A05FBD5FAAAE82918A9962F0B93B855F97993EC975EEAA80D740ADBF4FF747359D041D5C33EA71D281E446B14773BCA97B43A23FB801676BD207A436C6481F1D2B9078717461A5B9D32E688F87748544523B524B0D57D5EA77A2775D2ECFA032CFBDBF52FB3786160279004E57AE6AF874E7303CE53299CCC041C7BC308D82A5698F3A8D0C38271AE35F8E9DBFBB694B5C803D89F7AE435DE236D525F54759B65E372FCD68EF20FA7111F9E4AFF73";
    private static readonly BigInteger N = BigInteger.Parse("0" + N_hex, System.Globalization.NumberStyles.HexNumber);
    private static readonly BigInteger g = new BigInteger(2);
    
    private readonly BigInteger _k;

    public NativeSrpServer()
    {
        _k = ComputeK();
    }
    
    public (BigInteger b, BigInteger B) GenerateChallenge(BigInteger v)
    {
        // b = random(), at least 256 bits 
        var bBytes = new byte[32];
        RandomNumberGenerator.Fill(bBytes);
        var b = new BigInteger(bBytes, isUnsigned: true, isBigEndian: true);

        // B = (k * v + g^b) % N
        var term1 = (_k * v) % N;
        var term2 = BigInteger.ModPow(g, b, N);
        var B = (term1 + term2) % N;

        return (b, B);
    }
    
    public BigInteger ComputePremasterSecret(BigInteger A, BigInteger b, BigInteger v, BigInteger B)
    {
        if (A % N == 0)
        {
            throw new Exception("Illegal parameter: A % N == 0");
        }

        // u = H(PAD(A) | PAD(B))
        var u = ComputeU(A, B);

        // S = (A * v^u) ^ b % N
        var vToU = BigInteger.ModPow(v, u, N);
        var baseVal = (A * vToU) % N;
        var S = BigInteger.ModPow(baseVal, b, N);

        return S;
    }
    
    public byte[] ComputeExpectedClientProof(string username, byte[] salt, BigInteger A, BigInteger B, BigInteger S)
    {
        using var hashAlg = SHA256.Create();

        // H(N) xor H(g)
        var hN = hashAlg.ComputeHash(N.ToByteArray(isUnsigned: true, isBigEndian: true));
        var hG = hashAlg.ComputeHash(g.ToByteArray(isUnsigned: true, isBigEndian: true));
        var xorHash = new byte[hN.Length];
        for (var i = 0; i < hN.Length; i++) xorHash[i] = (byte)(hN[i] ^ hG[i]);

        var hI = hashAlg.ComputeHash(Encoding.UTF8.GetBytes(username));
        var K = hashAlg.ComputeHash(S.ToByteArray(isUnsigned: true, isBigEndian: true));

        // M1 = H(H(N) xor H(g), H(I), s, A, B, K)
        using var stream = new MemoryStream();
        stream.Write(xorHash);
        stream.Write(hI);
        stream.Write(salt);
        stream.Write(A.ToByteArray(isUnsigned: true, isBigEndian: true));
        stream.Write(B.ToByteArray(isUnsigned: true, isBigEndian: true));
        stream.Write(K);

        return hashAlg.ComputeHash(stream.ToArray());
    }
    
    public byte[] ComputeServerProof(BigInteger A, byte[] clientProofM1, BigInteger S)
    {
        using var hashAlg = SHA256.Create();
        var K = hashAlg.ComputeHash(S.ToByteArray(isUnsigned: true, isBigEndian: true));

        // M2 = H(A, M1, K)
        using var stream = new MemoryStream();
        stream.Write(A.ToByteArray(isUnsigned: true, isBigEndian: true));
        stream.Write(clientProofM1);
        stream.Write(K);

        return hashAlg.ComputeHash(stream.ToArray());
    }

    // k = Hash(N | PAD(g))
    private BigInteger ComputeK()
    {
        var nBytes = N.ToByteArray(isUnsigned: true, isBigEndian: true);
        var gPad = Pad(g);

        using var hashAlg = SHA256.Create();
        var hash = hashAlg.ComputeHash(Concat(nBytes, gPad));
        
        return new BigInteger(hash, isUnsigned: true, isBigEndian: true);
    }

    // u = Hash(PAD(A) | PAD(B))
    private BigInteger ComputeU(BigInteger A, BigInteger B)
    {
        using var hashAlg = SHA256.Create();
        var hash = hashAlg.ComputeHash(Concat(Pad(A), Pad(B)));
        
        return new BigInteger(hash, isUnsigned: true, isBigEndian: true);
    }

    private byte[] Pad(BigInteger x)
    {
        // N Length padding is 256 bytes
        int nByteLength = N.GetByteCount(isUnsigned: true);
        var bytes = x.ToByteArray(isUnsigned: true, isBigEndian: true);
        
        if (bytes.Length == nByteLength) return bytes;
        if (bytes.Length > nByteLength) throw new Exception("Value too large to pad");

        var padded = new byte[nByteLength];
        Buffer.BlockCopy(bytes, 0, padded, nByteLength - bytes.Length, bytes.Length);
        
        return padded;
    }

    private static byte[] Concat(byte[] a, byte[] b)
    {
        var result = new byte[a.Length + b.Length];
        Buffer.BlockCopy(a, 0, result, 0, a.Length);
        Buffer.BlockCopy(b, 0, result, a.Length, b.Length);
        return result;
    }
}