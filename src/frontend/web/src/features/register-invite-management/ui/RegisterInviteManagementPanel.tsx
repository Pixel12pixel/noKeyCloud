import {useCallback, useEffect, useState} from "react";
import {QRCodeSVG} from "qrcode.react";
import {Eye, EyeOff, Trash2, Copy, Clock, QrCode} from "lucide-react";
import {Button} from "@/shared/ui/button";
import {toast} from "sonner";
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from "@/shared/ui/select";
import {
    Table,
    TableBody,
    TableCell,
    TableHead,
    TableHeader,
    TableRow,
} from "@/shared/ui/table";
import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogHeader,
    DialogTitle,
} from "@/shared/ui/dialog";
import {backendBaseUrl} from "@/shared/config";
import type {RegisterInviteResponse} from "@/entities/register-invite";

function CountdownTimer({expiresAt}: { expiresAt: string | null }) {
    const [timeLeft, setTimeLeft] = useState("");

    useEffect(() => {
        if (!expiresAt) {
            setTimeLeft("Never Expires");
            return;
        }

        const calculateTime = () => {
            const diff = new Date(expiresAt).getTime() - new Date().getTime();

            if (diff <= 0) return "Expired";

            const days = Math.floor(diff / (1000 * 60 * 60 * 24));
            const hours = Math.floor((diff % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
            const minutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60));
            const seconds = Math.floor((diff % (1000 * 60)) / 1000);

            let timeString = "";
            if (days > 0) timeString += `${days} days `;
            if (hours > 0 || days > 0) timeString += `${hours} hrs `;
            if (minutes > 0) timeString += `${minutes} min `;
            timeString += `${seconds} sec`;

            return timeString.trim();
        };

        setTimeLeft(calculateTime());

        const timer = setInterval(() => {
            setTimeLeft(calculateTime());
        }, 1000);

        return () => clearInterval(timer);
    }, [expiresAt]);

    return (
        <span className={timeLeft === "Expired" ? "text-destructive font-medium" : "font-mono"}>
      {timeLeft}
    </span>
    );
}

export function RegisterInviteManagementPanel() {
    const [activeCodes, setActiveCodes] = useState<RegisterInviteResponse[]>([]);
    const [visibleCodes, setVisibleCodes] = useState<Set<string>>(new Set());
    const [isGenerating, setIsGenerating] = useState(false);
    const [isLoading, setIsLoading] = useState(true);
    const [duration, setDuration] = useState("24");
    const [selectedQrCode, setSelectedQrCode] = useState<string | null>(null);

    const fetchInvites = useCallback(async () => {
        try {
            const res = await fetch(`${backendBaseUrl}/api/Admin/active-register-invites`, {
                credentials: "include"
            });
            if (res.ok) {
                const data = (await res.json()) as RegisterInviteResponse[];
                setActiveCodes(data);
            } else {
                toast.error("Failed to load active invites.");
            }
        } catch (error) {
            toast.error("Network error while loading invites.");
        } finally {
            setIsLoading(false);
        }
    }, []);

    useEffect(() => {
        void fetchInvites();
    }, [fetchInvites]);

    const handleDurationChange = (value: string | null) => {
        setDuration(value ?? "24h");
    };

    const toggleVisibility = (id: string) => {
        setVisibleCodes((prev) => {
            const next = new Set(prev);
            if (next.has(id)) next.delete(id);
            else next.add(id);
            return next;
        });
    };

    const handleRevoke = async (id: string) => {
        try {
            const res = await fetch(`${backendBaseUrl}/api/Admin/revoke-invite/${id}`, {
                method: "DELETE",
                credentials: "include"
            });

            if (res.ok) {
                setActiveCodes((prev) => prev.filter((c) => c.id !== id));
                toast.success("Invite code revoked.");
            } else {
                toast.error("Failed to revoke invite.");
            }
        } catch (error) {
            toast.error("Network error while revoking invite.");
        }
    };

    const handleGenerate = async () => {
        setIsGenerating(true);
        try {
            const expirationHours = parseInt(duration, 10);

            const res = await fetch(`${backendBaseUrl}/api/Admin/generate-register-invite`, {
                method: "POST",
                headers: {"Content-Type": "application/json"},
                credentials: "include",
                body: JSON.stringify({expirationHours})
            });

            if (res.ok) {
                const generatedInvite = (await res.json()) as RegisterInviteResponse;
                await fetchInvites();
                setSelectedQrCode(generatedInvite.code);
                toast.success("New invite code generated!");
            } else {
                toast.error("Failed to generate code.");
            }
        } catch {
            toast.error("Network error while generating code.");
        } finally {
            setIsGenerating(false);
        }
    };

    const handleCopyLink = (code: string) => {
        const inviteUrl = `${window.location.origin}/register?code=${code}`;
        navigator.clipboard.writeText(inviteUrl).then();
        toast.success("Invite link copied to clipboard!");
    };

    return (
        <div className="space-y-6">
            <div className="flex flex-col gap-4 rounded-lg border bg-card p-6 shadow-sm">
                <label className="font-medium text-2xl">Generate Invite</label>
                <div className="flex flex-col sm:flex-row items-end justify-start gap-4">
                    <div className="flex flex-col gap-2 w-full sm:w-auto">
                        <label className="text-sm font-medium flex items-center gap-2">
                            <Clock className="w-4 h-4 text-muted-foreground"/>
                            Valid Duration
                        </label>
                        <Select value={duration} onValueChange={handleDurationChange}>
                            <SelectTrigger className="w-full sm:w-30">
                                <SelectValue placeholder="Select duration">
                                    {duration === "1" ? "1 Hour" :
                                        duration === "24" ? "24 Hours" :
                                            duration === "168" ? "7 Days" : ""}
                                </SelectValue>
                            </SelectTrigger>
                            <SelectContent>
                                <SelectItem value="1">1 Hour</SelectItem>
                                <SelectItem value="24">24 Hours</SelectItem>
                                <SelectItem value="168">7 Days</SelectItem>
                            </SelectContent>
                        </Select>
                    </div>

                    <Button onClick={handleGenerate} disabled={isGenerating} className="w-full sm:w-auto">
                        {isGenerating ? "Generating..." : "Generate Invite"}
                    </Button>
                </div>
            </div>

            <div className="rounded-md border bg-card shadow-sm overflow-hidden">
                <Table>
                    <TableHeader>
                        <TableRow>
                            <TableHead className="w-62.5">Invite Code</TableHead>
                            <TableHead>Created</TableHead>
                            <TableHead>Expires</TableHead>
                            <TableHead className="text-right">Actions</TableHead>
                        </TableRow>
                    </TableHeader>
                    <TableBody>
                        {isLoading ? (
                            <TableRow>
                                <TableCell colSpan={3} className="text-center text-muted-foreground h-24">
                                    Loading invites...
                                </TableCell>
                            </TableRow>
                        ) : activeCodes.length === 0 ? (
                            <TableRow>
                                <TableCell colSpan={3} className="text-center text-muted-foreground h-24">
                                    No active invite codes found.
                                </TableCell>
                            </TableRow>
                        ) : (
                            activeCodes.map((item) => {
                                const isVisible = visibleCodes.has(item.id);
                                const prefix = item.code.substring(0, 5);
                                const secret = item.code.substring(5);

                                const createdDate = new Date(item.createdAt);
                                const formattedDate = createdDate.toLocaleDateString(undefined, {
                                    month: 'short',
                                    day: 'numeric',
                                    year: 'numeric'
                                });
                                const formattedTime = createdDate.toLocaleTimeString(undefined, {
                                    hour: '2-digit',
                                    minute: '2-digit'
                                });

                                return (
                                    <TableRow key={item.id}>
                                        <TableCell>
                                            <div className="flex items-center font-mono text-sm">
                                                {isVisible ? (
                                                    <div className="flex items-center group w-full">
                                                        <span className="select-text cursor-text text-foreground">
                                                            {item.code}
                                                        </span>
                                                        <button
                                                            onClick={() => toggleVisibility(item.id)}
                                                            className="ml-2 p-1 rounded hover:bg-muted/50 text-muted-foreground/50 hover:text-muted-foreground transition-colors"
                                                            title="Click to hide"
                                                        >
                                                            <EyeOff className="w-4 h-4"/>
                                                        </button>
                                                    </div>
                                                ) : (
                                                    <div className="flex items-center">
                                                        <span>{prefix}</span>
                                                        <button
                                                            onClick={() => toggleVisibility(item.id)}
                                                            className="relative flex items-center group cursor-pointer overflow-hidden"
                                                            title="Click to reveal"
                                                        >
                                                            <span
                                                                className="text-muted-foreground blur-xs select-none opacity-70 group-hover:blur-[3px] transition-all">
                                                                {"0".repeat(secret.length)}
                                                            </span>
                                                            <div
                                                                className="absolute inset-0 flex items-center justify-center">
                                                                <Eye
                                                                    className="w-4 h-4 text-foreground/70 group-hover:text-foreground drop-shadow-md transition-colors"/>
                                                            </div>
                                                        </button>
                                                    </div>
                                                )}
                                            </div>
                                        </TableCell>

                                        <TableCell>
                                            <div className="flex flex-col">
                                                <span className="text-sm font-medium">{formattedDate}</span>
                                                <span className="text-xs text-muted-foreground">{formattedTime}</span>
                                            </div>
                                        </TableCell>

                                        <TableCell className="text-muted-foreground text-sm">
                                            <CountdownTimer expiresAt={item.expiresAt}/>
                                        </TableCell>

                                        <TableCell className="text-right space-x-2">
                                            <Button
                                                variant="ghost"
                                                size="icon"
                                                onClick={() => setSelectedQrCode(item.code)}
                                                title="Show QR Code"
                                            >
                                                <QrCode className="w-4 h-4"/>
                                            </Button>
                                            <Button
                                                variant="ghost"
                                                size="icon"
                                                onClick={() => handleCopyLink(item.code)}
                                                title="Copy Link"
                                            >
                                                <Copy className="w-4 h-4"/>
                                            </Button>
                                            <Button
                                                variant="ghost"
                                                size="icon"
                                                className="text-destructive hover:bg-destructive/10 hover:text-destructive"
                                                onClick={() => handleRevoke(item.id)}
                                                title="Revoke Code"
                                            >
                                                <Trash2 className="w-4 h-4"/>
                                            </Button>
                                        </TableCell>
                                    </TableRow>
                                );
                            })
                        )}
                    </TableBody>
                </Table>
            </div>

            <Dialog open={!!selectedQrCode} onOpenChange={(open) => !open && setSelectedQrCode(null)}>
                <DialogContent className="sm:max-w-md flex flex-col items-center text-center">
                    <DialogHeader>
                        <DialogTitle className="text-xl">Invite Code Ready!</DialogTitle>
                        <DialogDescription>
                            Share this QR code or link with the new user.
                        </DialogDescription>
                    </DialogHeader>

                    {selectedQrCode && (
                        <div className="flex flex-col items-center gap-6 py-4">
                            <div className="bg-white p-4 rounded-xl shadow-sm border">
                                <QRCodeSVG
                                    value={`${window.location.origin}/register?code=${selectedQrCode}`}
                                    size={200}
                                    level="H"
                                />
                            </div>

                            <div className="w-full space-y-2">
                                <p className="font-mono text-lg font-bold tracking-wider select-all">{selectedQrCode}</p>
                                <Button
                                    className="w-full"
                                    onClick={() => handleCopyLink(selectedQrCode)}
                                >
                                    <Copy className="w-4 h-4 mr-2"/>
                                    Copy Registration Link
                                </Button>
                            </div>
                        </div>
                    )}
                </DialogContent>
            </Dialog>
        </div>
    );
}