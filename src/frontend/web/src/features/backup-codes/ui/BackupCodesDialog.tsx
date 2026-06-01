import {useState, useEffect} from "react";
import {AlertTriangle, Copy, Download, Check, Loader2} from "lucide-react";
import {Button} from "@/shared/ui/button";
import {toast} from "sonner";
import {
    AlertDialog,
    AlertDialogContent,
    AlertDialogDescription,
    AlertDialogFooter,
    AlertDialogHeader,
    AlertDialogTitle,
} from "@/shared/ui/alert-dialog";

interface BackupCodesDialogProps {
    open: boolean;
    onAcknowledge: () => void;
}

export function BackupCodesDialog({
                                      open,
                                      onAcknowledge,
                                  }: BackupCodesDialogProps) {
    const [codes, setCodes] = useState<string[]>([]);
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const [hasCopied, setHasCopied] = useState(false);
    const [hasDownloaded, setHasDownloaded] = useState(false);
    const [isSubmitting, setIsSubmitting] = useState(false);

    useEffect(() => {
        if (open && codes.length === 0) {
            fetchBackupCodes();
        }
    }, [open]);

    const fetchBackupCodes = async () => {
        setIsLoading(true);
        setError(null);
        try {
            const mockCodes = [
                "8f92-a1b3", "c4d5-e6f7", "g8h9-i0j1", "k2l3-m4n5", "o6p7-q8r9",
                "s0t1-u2v3", "w4x5-y6z7", "a8b9-c0d1", "e2f3-g4h5", "i6j7-k8l9"
            ];

            setCodes(mockCodes);


            // TODO: Implement real API call to fetch backup codes
            /*
            const response = await fetch("");

            if (!response.ok) {
                throw new Error("Failed to fetch backup codes");
            }

            const data = await response.json();
            setCodes(data.codes);
            */

        } catch (err) {
            setError(err instanceof Error ? err.message : "An unknown error occurred");
            toast.error("Failed to load backup codes.");
        } finally {
            setIsLoading(false);
        }
    };

    const handleCopy = () => {
        navigator.clipboard.writeText(codes.join("\n"));
        setHasCopied(true);
        toast.success("Backup codes copied to clipboard");
        setTimeout(() => setHasCopied(false), 2000);
    };

    const handleDownload = () => {
        const blob = new Blob([codes.join("\n")], {type: "text/plain"});
        const url = URL.createObjectURL(blob);
        const a = document.createElement("a");
        a.href = url;
        a.download = "nokeycloud-backup-codes.txt";
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        URL.revokeObjectURL(url);

        setHasDownloaded(true);
        toast.success("Backup codes downloaded");
    };

    const handleComplete = async () => {
        if (!hasCopied && !hasDownloaded) {
            toast.error("Please copy or download your codes first.");
            return;
        }

        setIsSubmitting(true);
        try {
            // TODO: Call backend to confirm the user has seen the codes
            onAcknowledge();
        } catch (error) {
            toast.error("Failed to verify. Please try again.");
        } finally {
            setIsSubmitting(false);
        }
    };

    return (
        <AlertDialog open={open}>
            <AlertDialogContent className="sm:max-w-125">
                <AlertDialogHeader>
                    <AlertDialogTitle className="flex items-center gap-2 text-2xl text-destructive">
                        <AlertTriangle className="h-6 w-6"/>
                        Save Your Backup Codes
                    </AlertDialogTitle>
                    <AlertDialogDescription className="text-base text-foreground">
                        These codes are the <strong>only way</strong> to regain access to your encrypted files. We will
                        only show them to you once.
                    </AlertDialogDescription>
                </AlertDialogHeader>

                <div className="my-4 rounded-md border bg-muted/50 p-4 min-h-40 flex flex-col justify-center">
                    {isLoading ? (
                        <div className="flex flex-col items-center justify-center text-muted-foreground gap-2">
                            <Loader2 className="h-6 w-6 animate-spin"/>
                            <span className="text-sm">Generating secure codes...</span>
                        </div>
                    ) : error ? (
                        <div className="flex flex-col items-center justify-center gap-3 text-destructive">
                            <span className="text-sm">{error}</span>
                            <Button variant="outline" size="sm" onClick={fetchBackupCodes}>
                                Try Again
                            </Button>
                        </div>
                    ) : (
                        <div className="grid grid-cols-2 gap-4 text-center font-mono text-sm tracking-wider">
                            {codes.map((code, index) => (
                                <div key={index} className="rounded bg-background py-2 border shadow-sm select-all">
                                    {code}
                                </div>
                            ))}
                        </div>
                    )}
                </div>

                <div className="flex flex-col sm:flex-row gap-3 mb-4">
                    <Button
                        variant="outline"
                        className="flex-1"
                        onClick={handleCopy}
                        disabled={isLoading || !!error || codes.length === 0}
                    >
                        {hasCopied ? <Check className="mr-2 h-4 w-4 text-green-500"/> :
                            <Copy className="mr-2 h-4 w-4"/>}
                        {hasCopied ? "Copied!" : "Copy to Clipboard"}
                    </Button>
                    <Button
                        variant="outline"
                        className="flex-1"
                        onClick={handleDownload}
                        disabled={isLoading || !!error || codes.length === 0}
                    >
                        {hasDownloaded ? <Check className="mr-2 h-4 w-4 text-green-500"/> :
                            <Download className="mr-2 h-4 w-4"/>}
                        Download .txt
                    </Button>
                </div>

                <AlertDialogFooter>
                    <Button
                        onClick={handleComplete}
                        disabled={isLoading || isSubmitting || (!hasCopied && !hasDownloaded)}
                        className="w-full"
                    >
                        I have securely saved these codes
                    </Button>
                </AlertDialogFooter>
            </AlertDialogContent>
        </AlertDialog>
    );
}