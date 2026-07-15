import { backendBaseUrl } from '@/shared/config';

let csrfToken: string | null = null;
let csrfPromise: Promise<string> | null = null;

type CsrfResponse = {
    token: string;
};

export const fetchCsrfToken = async (): Promise<string> => {
    if (csrfPromise) {
        return csrfPromise;
    }

    csrfPromise = fetch(`${backendBaseUrl}/api/Authenticate/csrf`, {
        method: 'GET',
        credentials: 'include'
    })
        .then(async response => {
            if (!response.ok) {
                throw new Error(`Failed to fetch CSRF token: ${response.status}`);
            }

            const data: CsrfResponse = await response.json();

            if (!data.token) {
                throw new Error('Backend did not return a CSRF token');
            }

            csrfToken = data.token;

            return csrfToken;
        })
        .finally(() => {
            csrfPromise = null;
        });

    return csrfPromise;
};

export const getCsrfToken = async (): Promise<string> => {
    if (csrfToken) {
        return csrfToken;
    }

    return fetchCsrfToken();
};

export const clearCsrfToken = (): void => {
    csrfToken = null;
};