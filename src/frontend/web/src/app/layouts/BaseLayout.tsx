import { Outlet } from "react-router-dom";
import { Header } from "@/widgets/header/ui/Header";
import { Toaster } from "@/shared/ui/sonner";
import { Footer } from "@/widgets/footer/ui/Footer";
import { useEffect } from "react";
import { initializeAuth } from "@/entities/session/model/authStore";
import { AdminSetupDialog} from "@/features/admin-setup/ui/AdminSetupDialog";
import { useSystemSetup} from "@/entities/system/model/useSystemSetup";

export function BaseLayout() {
    const { needsSetup, setNeedsSetup } = useSystemSetup();

    useEffect(() => {
        void initializeAuth();
    }, []);

    return (
        <div className="relative min-h-screen flex flex-col">

            <AdminSetupDialog
                isOpen={needsSetup}
                onComplete={() => setNeedsSetup(false)}
            />

            <Header />

            <main className="flex-1 flex flex-col">
                <Outlet />
            </main>

            <Footer />

            <Toaster richColors position="bottom-right" />
        </div>
    );
}