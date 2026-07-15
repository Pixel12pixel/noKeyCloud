import { backendBaseUrl } from "@/shared/config";
import {customFetch} from "@/shared/api";

export interface UserProfileResponse {
    userId: string;
    username: string;
    email: string;
    rootFolderId: string;
    rootFolderKey: string;
    isAdmin?: boolean;
}

export async function fetchCurrentUser(): Promise<UserProfileResponse | null> {
    const response = await customFetch(`${backendBaseUrl}/api/Users/me`, {
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