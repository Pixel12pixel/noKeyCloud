const PBKDF2_ITERATIONS = 600000;
const IV_LENGTH = 12;
const SALT_LENGTH = 16;

export function generateSalt(): Uint8Array {
    return crypto.getRandomValues(new Uint8Array(SALT_LENGTH));
}

export async function generateAesKey(): Promise<CryptoKey> {
    return await crypto.subtle.generateKey(
        { name: "AES-GCM", length: 256 },
        true,
        ["encrypt", "decrypt"]
    );
}

export async function deriveKey(password: string, salt: Uint8Array): Promise<CryptoKey> {
    const encoder = new TextEncoder();
    const passwordKey = await crypto.subtle.importKey(
        "raw",
        encoder.encode(password),
        { name: "PBKDF2" },
        false,
        ["deriveKey"]
    );

    return await crypto.subtle.deriveKey(
        {
            name: "PBKDF2",
            salt: salt,
            iterations: PBKDF2_ITERATIONS,
            hash: "SHA-512",
        } as Pbkdf2Params,
        passwordKey,
        { name: "AES-GCM", length: 256 },
        true,
        ["encrypt", "decrypt"]
    );
}

export async function exportKey(key: CryptoKey): Promise<Uint8Array> {
    const exported = await crypto.subtle.exportKey("raw", key);
    return new Uint8Array(exported);
}

export async function importKey(rawKey: Uint8Array): Promise<CryptoKey> {
    return await crypto.subtle.importKey(
        "raw",
        rawKey as unknown as BufferSource,
        { name: "AES-GCM" },
        true,
        ["encrypt", "decrypt"]
    );
}

export async function encryptBytes(key: CryptoKey, data: Uint8Array): Promise<Uint8Array> {
    const iv = crypto.getRandomValues(new Uint8Array(IV_LENGTH));
    const encryptedBuffer = await crypto.subtle.encrypt(
        { name: "AES-GCM", iv: iv },
        key,
        data as unknown as BufferSource
    );

    const encryptedBytes = new Uint8Array(encryptedBuffer);
    const combined = new Uint8Array(iv.length + encryptedBytes.length);
    combined.set(iv, 0);
    combined.set(encryptedBytes, iv.length);

    return combined;
}

export async function decryptBytes(key: CryptoKey, encryptedData: Uint8Array): Promise<Uint8Array> {
    const iv = encryptedData.slice(0, IV_LENGTH);
    const ciphertext = encryptedData.slice(IV_LENGTH);

    const decryptedBuffer = await crypto.subtle.decrypt(
        { name: "AES-GCM", iv: iv },
        key,
        ciphertext
    );

    return new Uint8Array(decryptedBuffer);
}

export async function encryptString(key: CryptoKey, text: string): Promise<Uint8Array> {
    const encoder = new TextEncoder();
    return await encryptBytes(key, encoder.encode(text));
}

export async function decryptString(key: CryptoKey, encryptedData: Uint8Array): Promise<string> {
    const decryptedBytes = await decryptBytes(key, encryptedData);
    const decoder = new TextDecoder();
    return decoder.decode(decryptedBytes);
}
