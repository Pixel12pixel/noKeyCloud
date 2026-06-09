import { ThemeToggle } from "@/features/theme-switcher";
import { Link, useLocation, useNavigate } from "react-router-dom";
import { Button } from "@/shared/ui/button";
import { useAuth } from "@/entities/session";
import type {ReactNode} from "react";

interface HeaderProps {
    profileMenuSlot?: ReactNode;
}

export function Header({ profileMenuSlot }: HeaderProps) {
    const navigate = useNavigate();
    const location = useLocation();
    const auth = useAuth();

    const isLoginPage = location.pathname === "/login";

    return (
        <header className="flex items-center justify-between p-4 border-b">
            <Link to="/" className="text-xl font-bold hover:opacity-80 transition-opacity">
                noKeyCloud
            </Link>

            <div className="flex items-center gap-3">
                <ThemeToggle />
                {auth.status === "loading" ? (
                    <div className="h-8 w-8 rounded-full bg-muted animate-pulse" />
                ) : auth.status === "authenticated" ? (
                    profileMenuSlot
                ) : isLoginPage ? null : (
                    <Button onClick={() => navigate("/login")}>Log in</Button>
                )}
            </div>
        </header>
    );
}