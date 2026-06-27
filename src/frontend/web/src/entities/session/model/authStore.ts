import {fetchCurrentUser, type UserProfileResponse} from "@/shared/api";
import {decryptBytes, exportKey, importKey} from "@/shared/security";
import {bytesToBase64, base64ToBytes} from "@/shared/lib";
import {vaultKeys} from "@/entities/folder";
import {backendBaseUrl} from "@/shared/config";

type AuthStatus = "loading" | "authenticated" | "guest";

export type AuthState = {
    status: AuthStatus;
    user: UserProfileResponse | null;
    rootFolderId: string | null;
    masterKey: CryptoKey | null;
};

let state: AuthState = {status: "loading", user: null, rootFolderId: null, masterKey: null};
const listeners = new Set<() => void>();
let initPromise: Promise<void> | null = null;

function setState(next: AuthState) {
    state = next;
    listeners.forEach((listener) => listener());
}

export function getAuthState() {
    return state;
}

export function subscribeAuth(listener: () => void) {
    listeners.add(listener);
    return () => listeners.delete(listener);
}

export function setMasterKey(key: CryptoKey | null) {
    setState({...state, masterKey: key});

    if (key) {
        exportKey(key).then(rawKey => {
            localStorage.setItem("MK", bytesToBase64(rawKey));
        }).catch(err => console.error("Failed to save mk to storage", err));
    } else {
        localStorage.removeItem("MK");
        vaultKeys.clear();
    }
}

export function setGuest() {
    setState({status: "guest", user: null, rootFolderId: null, masterKey: null});
    localStorage.removeItem("MK");
    vaultKeys.clear();
}

export async function refreshAuth() {
    let currentMasterKey = state.masterKey;

    if (!currentMasterKey) {
        const storedKeyBase64 = localStorage.getItem("MK");
        if (storedKeyBase64) {
            try {
                const rawKey = base64ToBytes(storedKeyBase64);
                currentMasterKey = await importKey(rawKey);
                state.masterKey = currentMasterKey;
            } catch (e) {
                console.error("Failed to restore mk from storage", e);
                localStorage.removeItem("MK");
                vaultKeys.clear();
            }
        }
    }

    setState({status: "loading", user: state.user, rootFolderId: state.rootFolderId, masterKey: currentMasterKey});

    let me = await fetchCurrentUser();

    if (!me) {
        try {
            const refreshResponse = await fetch(`${backendBaseUrl}/api/Authenticate/refresh`, {
                method: "POST",
                credentials: "include",
                headers: { "Content-Type": "application/json" }
            });

            if (refreshResponse.ok) {
                me = await fetchCurrentUser();
            }
        } catch (refreshErr) {
            console.error("Silent token refresh network error:", refreshErr);
        }
    }
    
    if (me && currentMasterKey) {
        try {
            if(me.rootFolderId && me.rootFolderKey) {
                const encryptedRootFolderKeyBytes = base64ToBytes(me.rootFolderKey);
                const decryptedRootFolderKey = await decryptBytes(currentMasterKey, encryptedRootFolderKeyBytes);
                const rootFolderKey = await importKey(decryptedRootFolderKey);

                vaultKeys.setKey(me.rootFolderId, rootFolderKey);
            }

            setState({
                status: "authenticated",
                user: me,
                rootFolderId: me.rootFolderId ?? null,
                masterKey: currentMasterKey
            });
        } catch (cryptoError) {
            console.error("Failed to decrypt root folder key", cryptoError);
            setGuest();
        }
    } else {
        setGuest();
    }
}

export async function initializeAuth() {
    if (initPromise) return initPromise;

    initPromise = refreshAuth().finally(() => {
        initPromise = null;
    });

    return initPromise;
}