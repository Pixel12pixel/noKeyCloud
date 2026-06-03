import { useState, useEffect } from "react";
import { backendBaseUrl } from "@/shared/config";

export function useSystemSetup() {
    const [needsSetup, setNeedsSetup] = useState(false);
    const [isLoading, setIsLoading] = useState(false);

    useEffect(() => {
        async function checkStatus() {
            try {
                const res = await fetch(`${backendBaseUrl}/api/System/setup-status`);
                if (res.ok) {
                    const data = await res.json();
                    setNeedsSetup(data.needsSetup);
                } else {
                    console.error("Backend returned an error:", await res.text());
                }
            } catch (err) {
                console.error("Failed to check system status.", err);
            } finally {
                setIsLoading(false);
            }
        }

        void checkStatus();
    }, []);

    return { needsSetup, setNeedsSetup, isLoading };
}
