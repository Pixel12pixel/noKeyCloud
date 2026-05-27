import { Outlet } from "react-router-dom";
import { Header } from "@/widgets/header/ui/Header";
import { Toaster } from "@/shared/ui/sonner";

export function BaseLayout() {
    return (
        <div className="relative min-h-screen flex flex-col">
            <Header />

            <main className="flex-1">
                <Outlet />
            </main>

            <Toaster richColors position="bottom-right" />
        </div>
    );
}