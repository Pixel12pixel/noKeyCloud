import {useSyncExternalStore} from "react";
import {getAuthState, subscribeAuth} from "./authStore";

export function useAuth() {
    return useSyncExternalStore(subscribeAuth, getAuthState, getAuthState);
}