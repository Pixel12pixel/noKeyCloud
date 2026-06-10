import { useState } from "react";
import { Lock } from "lucide-react";
import { Button } from "@/shared/ui/button";
import { Input } from "@/shared/ui/input";
import { Label } from "@/shared/ui/label";
import { toast } from "sonner";
import {
    AlertDialog,
    AlertDialogContent,
    AlertDialogDescription,
    AlertDialogFooter,
    AlertDialogHeader,
    AlertDialogTitle,
} from "@/shared/ui/alert-dialog";
import { prepareRegistration } from "@/shared/security";
import { backendBaseUrl } from "@/shared/config";
import { BackupCodesDialog } from "@/features/backup-codes";

interface AdminSetupDialogProps {
    isOpen: boolean;
    onComplete: () => void;
}

export function AdminSetupDialog({ isOpen, onComplete }: AdminSetupDialogProps) {
    const [email] = useState("admin@local.com");
    const [username] = useState("admin");
    const [password, setPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [error, setError] = useState("");
    const [recoveryCode, setRecoveryCode] = useState<string | null>(null);

    const handleSubmit = async (e: React.SyntheticEvent) => {
        e.preventDefault();
        setError("");

        if (password.length < 12) {
            setError("Password must be at least 12 characters long.");
            return;
        }
        if (password !== confirmPassword) {
            setError("Passwords do not match.");
            return;
        }

        try {
            setIsSubmitting(true);

            const cryptoPayload = await prepareRegistration(username, password);

            const response = await fetch(`${backendBaseUrl}/api/Authenticate/register`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({
                    email: email,
                    username: username,
                    salt: cryptoPayload.srpSaltBase64,
                    verifier: cryptoPayload.srpVerifierBase64,
                    encryptedMasterKey: cryptoPayload.encryptedMasterKeyBase64,
                    keySalt: cryptoPayload.keySaltBase64,
                    recoveryEncryptedMasterKey: cryptoPayload.recoveryEncryptedMasterKeyBase64,
                    inviteCode: null
                })
            });

            if (!response.ok) {
                const errorText = await response.text();
                setError(errorText || "Registration failed.");
                return;
            }

            toast.success("Admin account created successfully.");
            setRecoveryCode(cryptoPayload.recoveryPasswordPlaintext);

        } catch {
            setError("Failed to set up admin account. Please try again.");
        } finally {
            setIsSubmitting(false);
        }
    };

    if (recoveryCode) {
        return (
            <BackupCodesDialog
                open={true}
                codes={[recoveryCode]}
                username={username}
                onAcknowledge={() => {
                    setRecoveryCode(null);
                    onComplete();
                }}
            />
        );
    }

    return (
        <AlertDialog open={isOpen}>
            <AlertDialogContent
                className="sm:max-w-106.25"
            >
                <form onSubmit={handleSubmit}>
                    <AlertDialogHeader>
                        <AlertDialogTitle className="flex items-center gap-2 text-2xl">
                            <Lock className="h-6 w-6 text-primary" />
                            Initial Setup
                        </AlertDialogTitle>
                        <AlertDialogDescription>
                            Welcome to noKeyCloud. <br/>
                            Please set the master admin password. <br/>
                            Keep this safe it cannot be recovered.
                        </AlertDialogDescription>
                    </AlertDialogHeader>

                    <div className="grid gap-4 py-6">
                        {error && (
                            <div className="text-sm font-medium text-destructive bg-destructive/10 p-3 rounded-md">
                                {error}
                            </div>
                        )}

                        <div className="grid gap-2">
                            <Label htmlFor="admin-password">Master Password</Label>
                            <Input
                                id="admin-password"
                                type="password"
                                value={password}
                                onChange={(e) => setPassword(e.target.value)}
                                required
                                disabled={isSubmitting}
                                autoFocus
                            />
                        </div>

                        <div className="grid gap-2">
                            <Label htmlFor="confirm-password">Confirm Password</Label>
                            <Input
                                id="confirm-password"
                                type="password"
                                value={confirmPassword}
                                onChange={(e) => setConfirmPassword(e.target.value)}
                                required
                                disabled={isSubmitting}
                            />
                        </div>
                    </div>

                    <AlertDialogFooter>
                        <Button type="submit" className="w-full" disabled={isSubmitting}>
                            {isSubmitting ? "Securing system..." : "Complete Setup"}
                        </Button>
                    </AlertDialogFooter>
                </form>
            </AlertDialogContent>
        </AlertDialog>
    );
}
