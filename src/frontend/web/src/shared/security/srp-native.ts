// N = 2048 bit prime number from RFC 5054
const N_HEX = "AC6BDB41324A9A9BF166DE5E1389582FAF72B6651987EE07FC3192943DB56050A37329CBB4A099ED8193E0757767A13DD52312AB4B03310DCD7F48A9DA04FD50E8083969EDB767B0CF6095179A163AB3661A05FBD5FAAAE82918A9962F0B93B855F97993EC975EEAA80D740ADBF4FF747359D041D5C33EA71D281E446B14773BCA97B43A23FB801676BD207A436C6481F1D2B9078717461A5B9D32E688F87748544523B524B0D57D5EA77A2775D2ECFA032CFBDBF52FB3786160279004E57AE6AF874E7303CE53299CCC041C7BC308D82A5698F3A8D0C38271AE35F8E9DBFBB694B5C803D89F7AE435DE236D525F54759B65E372FCD68EF20FA7111F9E4AFF73";
export const N = BigInt("0x" + N_HEX);
export const g = 2n;

const encoder = new TextEncoder();

export function modPow(base: bigint, exponent: bigint, modulus: bigint): bigint {
    let result = 1n;
    base = base % modulus;
    while (exponent > 0n) {
        if (exponent % 2n === 1n) result = (result * base) % modulus;
        exponent = exponent / 2n;
        base = (base * base) % modulus;
    }
    return result;
}

export function bigIntToBytes(val: bigint): Uint8Array {
    let hex = val.toString(16);
    if (hex.length % 2 !== 0) hex = '0' + hex;
    const bytes = new Uint8Array(hex.length / 2);
    for (let i = 0; i < bytes.length; i++) {
        bytes[i] = parseInt(hex.slice(i * 2, i * 2 + 2), 16);
    }
    return bytes;
}

export function bytesToBigInt(bytes: Uint8Array): bigint {
    let hex = '';
    for (const b of bytes) hex += b.toString(16).padStart(2, '0');
    return hex === '' ? 0n : BigInt("0x" + hex);
}

export function pad(bytes: Uint8Array, len = 256): Uint8Array {
    if (bytes.length >= len) return bytes;
    const padded = new Uint8Array(len);
    padded.set(bytes, len - bytes.length);
    return padded;
}

export function bytesToBase64(bytes: Uint8Array): string {
    return btoa(String.fromCharCode(...bytes));
}
export function base64ToBytes(base64: string): Uint8Array {
    const binStr = atob(base64);
    const bytes = new Uint8Array(binStr.length);
    for (let i = 0; i < binStr.length; i++) bytes[i] = binStr.charCodeAt(i);
    return bytes;
}

export async function sha256(...dataParts: Uint8Array[]): Promise<Uint8Array> {
    const totalLength = dataParts.reduce((acc, part) => acc + part.length, 0);
    const combined = new Uint8Array(totalLength);
    let offset = 0;
    for (const p of dataParts) {
        combined.set(p, offset);
        offset += p.length;
    }
    const hash = await crypto.subtle.digest("SHA-256", combined);
    return new Uint8Array(hash);
}

export async function computeX(salt: Uint8Array, I: string, P: string): Promise<bigint> {
    const hIP = await sha256(encoder.encode(`${I}:${P}`));
    const hx = await sha256(salt, hIP);
    return bytesToBigInt(hx);
}