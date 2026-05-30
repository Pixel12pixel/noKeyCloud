import {User, Shield} from "lucide-react";
import {Button} from "@/shared/ui/button";
import {Input} from "@/shared/ui/input";
import {Label} from "@/shared/ui/label";
import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogHeader,
    DialogTitle,
} from "@/shared/ui/dialog";
import {
    Tabs,
    TabsContent,
    TabsList,
    TabsTrigger,
} from "@/shared/ui/tabs";
import {useAuth} from "@/entities/session/model/useAuth";

interface SettingsDialogProps {
    open: boolean;
    onOpenChange: (open: boolean) => void;
}

export function SettingsDialog({open, onOpenChange}: SettingsDialogProps) {
    const auth = useAuth();

    return (
        <Dialog open={open} onOpenChange={onOpenChange}>
            <DialogContent
                className="sm:max-w-300 h-[80vh] min-h-125 max-h-175 p-0 flex flex-col overflow-hidden gap-0">

                <div className="p-6 border-b">
                    <DialogHeader>
                        <DialogTitle className="text-2xl">Settings</DialogTitle>
                        <DialogDescription>
                            Manage your settings.
                        </DialogDescription>
                    </DialogHeader>
                </div>

                <Tabs defaultValue="account" orientation="vertical" className="flex-1 flex flex-col min-h-0">
                    <div className="flex flex-row flex-1 h-full overflow-hidden">
                        <TabsList
                            variant="line" className="flex flex-col w-48 h-full items-stretch justify-start gap-2 p-3 rounded-none">
                            <TabsTrigger value="account"
                                         className="justify-start data-[state=active]:bg-background data-[state=active]:shadow-sm">
                                <User className="w-4 h-4 mr-2"/>
                                Account
                            </TabsTrigger>
                            <TabsTrigger value="security"
                                         className="justify-start data-[state=active]:bg-background data-[state=active]:shadow-sm">
                                <Shield className="w-4 h-4 mr-2"/>
                                Security
                            </TabsTrigger>
                        </TabsList>

                        <div className="flex-1 overflow-y-auto p-6 bg-background">

                            <TabsContent value="account" className="m-0 outline-none space-y-6">
                                <div className="space-y-4">
                                    <h3 className="text-lg font-medium">My Account</h3>
                                    <div className="grid gap-2">
                                        <Label htmlFor="display-name">Username</Label>

                                        <Input id="display-name" defaultValue={auth.user?.username ?? ""} disabled/>
                                    </div>
                                    <Button>Save Account Changes</Button>
                                </div>
                            </TabsContent>

                            <TabsContent value="security" className="m-0 outline-none space-y-6">
                                <div className="space-y-4">
                                    <h3 className="text-lg font-medium">Security</h3>
                                    <div className="flex items-center justify-between rounded-lg border p-4">
                                        <div className="space-y-0.5">
                                            <Label className="text-base text-destructive">Master Password</Label>
                                            <p className="text-sm text-muted-foreground">
                                                Change the master password.
                                            </p>
                                        </div>
                                        <Button variant="destructive">Change Password</Button>
                                    </div>
                                </div>
                            </TabsContent>
                        </div>
                    </div>
                </Tabs>
            </DialogContent>
        </Dialog>
    );
}