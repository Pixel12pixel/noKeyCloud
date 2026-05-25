if (!import.meta.env.VITE_BACKEND_URL) {
    console.warn("VITE_BACKEND_URL is not defined in your environment variables!");
}

export const backendBaseUrl = import.meta.env.VITE_BACKEND_URL as string;