import {useEffect} from "react";
import {useNavigate} from "react-router-dom";
import {useAuth} from "@/entities/session/model/useAuth.ts";
import {Tabs, TabsContent, TabsList, TabsTrigger} from "@/shared/ui/tabs.tsx";
import {Link} from "lucide-react";
import {RegisterInviteManagementPanel} from "@/features/register-invite-management/ui/RegisterInviteManagementPanel";

export function AdminPanelPage() {
    const navigate = useNavigate();
    const auth = useAuth();

    useEffect(() => {
        document.title = "Admin - noKeyCloud";
    }, []);

    useEffect(() => {
        if (auth.status === "guest") {
            navigate("/login", {replace: true});
        }
        if (auth.status === "authenticated" && auth.user?.isAdmin !== true) {
            navigate("/", {replace: true});
        }
    }, [auth.status, navigate]);

    return (
        <Tabs defaultValue="register-invites" orientation="vertical" className="flex-1 flex flex-col min-h-0">
            <div className="flex flex-row flex-1 h-full overflow-hidden">
                <TabsList
                    variant="line"
                    className="flex flex-col w-48 h-full items-stretch justify-start gap-2 p-3 rounded-none">
                    <TabsTrigger value="register-invites"
                                 className="justify-start data-[state=active]:bg-background data-[state=active]:shadow-sm">
                        <Link className="w-4 h-4 mr-2"/>
                        Register Invites
                    </TabsTrigger>
                </TabsList>

                <div className="flex-1 overflow-y-auto p-6 bg-background">

                    <TabsContent value="register-invites" className="m-0 outline-none space-y-6">
                        <div className="space-y-4">
                            <h3 className="text-lg font-medium">Register Invites</h3>
                            <RegisterInviteManagementPanel />
                        </div>
                    </TabsContent>

                </div>
            </div>
        </Tabs>
    );
}