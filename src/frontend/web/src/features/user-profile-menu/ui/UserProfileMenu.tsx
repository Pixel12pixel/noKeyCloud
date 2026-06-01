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
import { toast } from "sonner";
import {useState} from "react";
import {SettingsDialog} from "@/widgets/settings/ui/SettingsDialog";

export function UserProfileMenu() {
    const navigate = useNavigate();
    const auth = useAuth();

    const [showSettings, setShowSettings] = useState(false);

    if (auth.status !== "authenticated" || !auth.user) return null;

    async function handleLogout() {
        try {
            await logoutUserApi();
            sessionStorage.removeItem("breadcrumbHistory");
            setGuest();
            toast.success("Logged out successfully");
            navigate("/login", { replace: true });
        } catch (err) {
            console.error("Logout failed:", err);
            toast.error("Logout failed. Please try again.");
        }
    }

    const initials = auth.user.username?.slice(0, 2).toUpperCase() ?? "U";

    return (
        <>
            <DropdownMenu>
                <DropdownMenuTrigger className="outline-none">
                    <Avatar className="h-8 w-8 hover:opacity-80 transition-opacity cursor-pointer">
                        <AvatarFallback>{initials}</AvatarFallback>
                    </Avatar>
                </DropdownMenuTrigger>
                <DropdownMenuContent align="end" sideOffset={8} className="w-max min-w-0 max-w-48 truncate">
                    <DropdownMenuGroup>
                        <DropdownMenuLabel>{auth.user.username}</DropdownMenuLabel>
                        <DropdownMenuSeparator/>

                        <DropdownMenuItem onClick={() => setShowSettings(true)}>
                            <Settings className="mr-2 h-4 w-4"/>
                            Settings
                        </DropdownMenuItem>

                        <DropdownMenuItem onClick={handleLogout}
                                          className="text-red-500 focus:text-red-500 focus:bg-red-500/10">
                            <LogOut className="mr-2 h-4 w-4"/>
                            Logout
                        </DropdownMenuItem>
                    </DropdownMenuGroup>
                </DropdownMenuContent>
            </DropdownMenu>

            <SettingsDialog open={showSettings} onOpenChange={setShowSettings}/>
        </>
    );
}