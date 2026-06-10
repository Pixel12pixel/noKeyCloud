import { backendBaseUrl } from '@/shared/config';
import {
    N, g, modPow, bytesToBigInt, bigIntToBytes, computeX,
    sha256, pad
} from './srp-native';
import {bytesToBase64, base64ToBytes} from '@/shared/lib';

export interface LoginInitResponse {
    salt: string;
    b: string;
    sessionId: string;
}

export interface LoginVerifyResponse {
    m2: string;
    rootFolderId: string;
    encryptedMasterKey: string;
    keySalt: string;
}

export const loginWithSRP = async (username: string, password: string) => {
    const aBytes = new Uint8Array(32);
    crypto.getRandomValues(aBytes);
    const a = bytesToBigInt(aBytes);
    const A = modPow(g, a, N);

    const ABase64 = bytesToBase64(bigIntToBytes(A));

    const initRes = await fetch(`${backendBaseUrl}/api/Authenticate/login/srp/init`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ username, email: username, a: ABase64 })
    });

    if (!initRes.ok) throw new Error('Invalid credentials');

    const { salt, b, sessionId } = (await initRes.json()) as LoginInitResponse;

    const sBytes = base64ToBytes(salt);
    const bBytes = base64ToBytes(b);
    const B = bytesToBigInt(bBytes);

    if (B % N === 0n) throw new Error('Server aborted: Safety check (B % N == 0) invoked.');


    // u = H(PAD(A) | PAD(B))
    const uHash = await sha256(pad(bigIntToBytes(A)), pad(bigIntToBytes(B)));
    const u = bytesToBigInt(uHash);

    // k = H(N | PAD(g))
    const kHash = await sha256(bigIntToBytes(N), pad(bigIntToBytes(g)));
    const k = bytesToBigInt(kHash);

    // x = H(s | H(I | ":" | P))
    const x = await computeX(sBytes, username, password);

    // S = (B - k * g^x) ^ (a + u * x) % N
    const gx = modPow(g, x, N);
    const kgx = (k * gx) % N;
    let base = (B - kgx) % N;
    if (base < 0n) base += N;
    const exp = a + u * x;
    const S = modPow(base, exp, N);

    const aBytesPad = pad(bigIntToBytes(A));
    const bBytesPad = pad(bigIntToBytes(B));
    const sBytesPad = pad(bigIntToBytes(S));

    // M1 = H(H(N) xor H(g), H(I), s, A, B, K)
    const K = await sha256(sBytesPad);

    const hN = await sha256(pad(bigIntToBytes(N)));
    const hG = await sha256(bigIntToBytes(g));
    const hNxorhG = new Uint8Array(32);
    for(let i = 0; i < 32; i++)
    {
        hNxorhG[i] = hN[i] ^ hG[i];
    }

    const hI = await sha256(new TextEncoder().encode(username));

    const M1 = await sha256(hNxorhG, hI, sBytes, aBytesPad, bBytesPad, K);

    const verifyRes = await fetch(`${backendBaseUrl}/api/Authenticate/login/srp/verify`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include',
        body: JSON.stringify({ sessionId, m1: bytesToBase64(M1) })
    });

    if (!verifyRes.ok) throw new Error('Invalid credentials');

    const authData = (await verifyRes.json()) as LoginVerifyResponse;

    // M2 = H(A, M1, K)
    const expectedM2 = await sha256(aBytesPad, M1, K);

    if (bytesToBase64(expectedM2) !== authData.m2) {
        throw new Error('Server backwards trust verification failed: possible Man In The Middle');
    }

    return authData;
};