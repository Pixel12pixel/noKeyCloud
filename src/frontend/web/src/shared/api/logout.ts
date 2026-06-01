import { backendBaseUrl } from '@/shared/config/backend';

export async function logoutUserApi(): Promise<void> {
    const response = await fetch(`${backendBaseUrl}/api/Authenticate/logout`, {
        method: 'POST',
        credentials: 'include',
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (!response.ok) {
        console.error('Logout failed on the backend, falling back to local clearance');
    }
}