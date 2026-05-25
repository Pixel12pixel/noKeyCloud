interface SessionData {
    userId: string;
    rootFolderId: string;
    accessTokenExpiresAt: string;
}

export function saveSession(session: SessionData) {
    localStorage.setItem("user_id", session.userId);
    localStorage.setItem("root_folder_id", session.rootFolderId);
    localStorage.setItem("access_token_expires_at", session.accessTokenExpiresAt);
}

export function clearSession() {
    localStorage.removeItem("user_id");
    localStorage.removeItem("root_folder_id");
    localStorage.removeItem("access_token_expires_at");
}

export function isSessionExpired(): boolean {
    const expiresAt = localStorage.getItem("access_token_expires_at");
    if (!expiresAt) return true;
    return Date.now() >= new Date(expiresAt).getTime();
}

export function getUserId(): string | null {
    return localStorage.getItem("user_id");
}