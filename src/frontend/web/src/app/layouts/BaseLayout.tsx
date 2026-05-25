import { Outlet } from "react-router-dom";
import { Header } from "@/widgets/header/ui/Header";

export function BaseLayout() {
    return (
        <div className="relative min-h-screen flex flex-col">
            <Header />

            <main className="flex-1">
                <Outlet />
            </main>
        </div>
    );
}