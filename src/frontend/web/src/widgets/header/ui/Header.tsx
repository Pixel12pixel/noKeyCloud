import { ThemeToggle } from "@/features/theme-switcher/ui/ThemeToggle";
import { Link, useLocation, useNavigate } from "react-router-dom";
import { Button } from "@/shared/ui/button";
import { UserProfileMenu } from "@/features/user-profile-menu/ui/UserProfileMenu";
import { useAuth } from "@/entities/session/model/useAuth";

export function Header() {
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
                    <UserProfileMenu />
                ) : isLoginPage ? null : (
                    <Button onClick={() => navigate("/login")}>Log in</Button>
                )}
            </div>
        </header>
    );
}