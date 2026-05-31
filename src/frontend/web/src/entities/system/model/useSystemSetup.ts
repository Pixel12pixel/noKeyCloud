import { useState, useEffect } from "react";

export function useSystemSetup() {
    const [needsSetup, setNeedsSetup] = useState(false);
    const [isLoading, setIsLoading] = useState(false);

    useEffect(() => {
        // TODO: Fetch from backend if the admin account exists
    }, []);

    return { needsSetup, setNeedsSetup, isLoading };
}
