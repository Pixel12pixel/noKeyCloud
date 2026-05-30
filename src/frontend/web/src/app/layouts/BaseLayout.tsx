import { Outlet } from "react-router-dom";
import { Header } from "@/widgets/header/ui/Header";
import { Toaster } from "@/shared/ui/sonner";
import { Footer } from "@/widgets/footer/ui/Footer";
import { useEffect } from "react";
import { initializeAuth } from "@/entities/session/model/authStore";

export function BaseLayout() {
    useEffect(() => {
        void initializeAuth();
    }, []);

    return (
        <div className="relative min-h-screen flex flex-col">
            <Header />

            <main className="flex-1 flex flex-col">
                <Outlet />
            </main>

            <Footer />

            <Toaster richColors position="bottom-right" />
        </div>
    );
}