import { Outlet } from "react-router-dom";
import { Header } from "@/widgets/header";
import { Toaster } from "@/shared/ui/sonner";
import { Footer } from "@/widgets/footer";
import { useEffect } from "react";
import { initializeAuth } from "@/entities/session";
import { AdminSetupDialog} from "@/features/admin-setup";
import { useSystemSetup} from "@/entities/system";
import { UserProfileMenu } from "@/widgets/user-profile-menu";

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

            <Header profileMenuSlot={<UserProfileMenu />} />

            <main className="flex-1 flex flex-col">
                <Outlet />
            </main>

            <Footer />

            <Toaster richColors position="bottom-right" />
        </div>
    );
}