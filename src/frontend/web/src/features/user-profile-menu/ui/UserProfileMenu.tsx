import {useNavigate} from "react-router-dom";
import {
    DropdownMenu, DropdownMenuContent,
    DropdownMenuGroup, DropdownMenuItem, DropdownMenuLabel, DropdownMenuSeparator, DropdownMenuTrigger
} from "@/shared/ui/dropdown-menu";
import {Avatar, AvatarFallback} from "@/shared/ui/avatar";
import {logoutUserApi} from "@/shared/api/logout";
import {setGuest} from "@/entities/session/model/authStore";
import {useAuth} from "@/entities/session/model/useAuth";
import {Settings, LogOut} from "lucide-react";

export function UserProfileMenu() {
    const navigate = useNavigate();
    const auth = useAuth();

    if (auth.status !== "authenticated" || !auth.user) return null;

    async function handleLogout() {
        await logoutUserApi();
        sessionStorage.removeItem("breadcrumbHistory");
        setGuest();
        navigate("/login", {replace: true});
    }

    const initials = auth.user.username?.slice(0, 2).toUpperCase() ?? "U";

    return (
        <DropdownMenu>
            <DropdownMenuTrigger className="outline-none">
                <Avatar className="h-8 w-8">
                    <AvatarFallback>{initials}</AvatarFallback>
                </Avatar>
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end" sideOffset={8} className="w-max min-w-0 max-w-48 truncate">
                <DropdownMenuGroup>
                    <DropdownMenuLabel>{auth.user.username}</DropdownMenuLabel>
                    <DropdownMenuSeparator/>
                    <DropdownMenuItem>
                        <Settings className="h-4 w-4"/>
                        Settings
                    </DropdownMenuItem>
                    <DropdownMenuItem onClick={handleLogout}>
                        <LogOut className="h-4 w-4"/>
                        Logout
                    </DropdownMenuItem>
                </DropdownMenuGroup>
            </DropdownMenuContent>
        </DropdownMenu>
    );
}