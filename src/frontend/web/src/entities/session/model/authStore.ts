import {fetchCurrentUser, type UserProfileResponse} from "@/shared/api/user";

type AuthStatus = "loading" | "authenticated" | "guest";

export type AuthState = {
    status: AuthStatus;
    user: UserProfileResponse | null;
    rootFolderId: string | null;
};

let state: AuthState = {status: "loading", user: null, rootFolderId: null};
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

export function setGuest() {
    setState({status: "guest", user: null, rootFolderId: null});
}

export async function refreshAuth() {
    setState({status: "loading", user: null, rootFolderId: null});

    const me = await fetchCurrentUser();
    if (me) {
        setState({
            status: "authenticated",
            user: me,
            rootFolderId: me.rootFolderId ?? null
        });
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