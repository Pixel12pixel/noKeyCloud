import { backendBaseUrl } from "@/shared/config";
import { setGuest } from "@/entities/session";

export async function customFetch(url: string, options: RequestInit = {}): Promise<Response> {
    options.credentials = options.credentials || "include";

    let response = await fetch(url, options);

    if (response.status === 401 && !url.includes("/api/Authenticate/refresh")) {
        try {
            const refreshRes = await fetch(`${backendBaseUrl}/api/Authenticate/refresh`, {
                method: "POST",
                credentials: "include",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({})
            });

            if (refreshRes.ok) {
                response = await fetch(url, options);
            } else {
                setGuest();
            }
        } catch (err) {
            console.error("Critical routing error during intercept refresh loop", err);
        }
    }

    return response;
}