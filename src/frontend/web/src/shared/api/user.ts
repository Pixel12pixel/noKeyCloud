import { backendBaseUrl } from "@/shared/config/backend";

export interface UserProfileResponse {
    userId: string;
    username: string;
    email: string;
    rootFolderId: string;
}

export async function fetchCurrentUser(): Promise<UserProfileResponse | null> {
    const response = await fetch(`${backendBaseUrl}/api/Users/me`, {
        method: "GET",
        credentials: "include",
        headers: {
            "Content-Type": "application/json"
        }
    });

    if (!response.ok) {
        return null;
    }

    return await response.json();
}