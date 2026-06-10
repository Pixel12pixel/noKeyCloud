import { generateSrpVerifier } from "./srp-native";
import {
    generateSalt,
    generateAesKey,
    deriveKey,
    exportKey,
    encryptBytes
} from "./crypto";
import {bytesToBase64} from "@/shared/lib";

export interface RegistrationPayload {
    srpSaltBase64: string;
    srpVerifierBase64: string;
    keySaltBase64: string;
    encryptedMasterKeyBase64: string;
    recoveryEncryptedMasterKeyBase64: string;
    recoveryPasswordPlaintext: string;
}

export async function prepareRegistration(username: string, password: string): Promise<RegistrationPayload> {
    const { saltBase64: srpSaltBase64, verifierBase64: srpVerifierBase64 } = await generateSrpVerifier(username, password);

    const masterKey = await generateAesKey();
    const exportedMasterKey = await exportKey(masterKey);

    const keySalt = generateSalt();
    const kek = await deriveKey(password, keySalt);

    const encryptedMasterKey = await encryptBytes(kek, exportedMasterKey);

    const recoveryBytes = crypto.getRandomValues(new Uint8Array(32));
    const recoveryPasswordPlaintext = bytesToBase64(recoveryBytes);

    const recoveryKek = await deriveKey(recoveryPasswordPlaintext, keySalt);
    const recoveryEncryptedMasterKey = await encryptBytes(recoveryKek, exportedMasterKey);

    return {
        srpSaltBase64,
        srpVerifierBase64,
        keySaltBase64: bytesToBase64(keySalt),
        encryptedMasterKeyBase64: bytesToBase64(encryptedMasterKey),
        recoveryEncryptedMasterKeyBase64: bytesToBase64(recoveryEncryptedMasterKey),
        recoveryPasswordPlaintext
    };
}