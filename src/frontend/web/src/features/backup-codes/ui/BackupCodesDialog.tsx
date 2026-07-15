import {useState} from "react";
import {AlertTriangle, Copy, Download, Check} from "lucide-react";
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
    codes: string[];
    username: string;
    onAcknowledge: () => void;
}

export function BackupCodesDialog({
                                      open,
                                      codes,
                                      username,
                                      onAcknowledge,
                                  }: BackupCodesDialogProps) {
    const [hasCopied, setHasCopied] = useState(false);
    const [hasDownloaded, setHasDownloaded] = useState(false);

    const handleCopy = () => {
        navigator.clipboard.writeText(codes.join("\n"));
        setHasCopied(true);
        toast.success("Recovery code copied to clipboard");
        setTimeout(() => setHasCopied(false), 2000);
    };

    const handleDownload = () => {
        const blob = new Blob([codes.join("\n")], {type: "text/plain"});
        const url = URL.createObjectURL(blob);
        const a = document.createElement("a");
        a.href = url;
        a.download = `nokeycloud-${username}-recovery-code.txt`;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        URL.revokeObjectURL(url);

        setHasDownloaded(true);
        toast.success("Recovery code downloaded");
    };

    const handleComplete = () => {
        if (!hasCopied && !hasDownloaded) {
            toast.error("Please copy or download your code first.");
            return;
        }
        onAcknowledge();
    };

    return (
        <AlertDialog open={open}>
            <AlertDialogContent className="sm:max-w-125">
                <AlertDialogHeader>
                    <AlertDialogTitle className="flex items-center gap-2 text-2xl text-destructive">
                        <AlertTriangle className="h-6 w-6"/>
                        Save Your Recovery Code
                    </AlertDialogTitle>
                    <AlertDialogDescription className="text-base text-foreground">
                        This code is the <strong>only way</strong> to regain access to your encrypted files if you forget your password. We will only show it to you once.
                    </AlertDialogDescription>
                </AlertDialogHeader>

                <div className="my-4 rounded-md border bg-muted/50 p-4 min-h-32 flex flex-col justify-center">
                    <div className="grid grid-cols-1 gap-4 text-center font-mono text-lg tracking-wider">
                        {codes.map((code, index) => (
                            <div key={index} className="rounded bg-background py-4 px-2 border shadow-sm select-all break-all">
                                {code}
                            </div>
                        ))}
                    </div>
                </div>

                <div className="flex flex-col sm:flex-row gap-3 mb-4">
                    <Button
                        variant="outline"
                        className="flex-1"
                        onClick={handleCopy}
                        disabled={codes.length === 0}
                    >
                        {hasCopied ? <Check className="mr-2 h-4 w-4 text-green-500"/> :
                            <Copy className="mr-2 h-4 w-4"/>}
                        {hasCopied ? "Copied!" : "Copy to Clipboard"}
                    </Button>
                    <Button
                        variant="outline"
                        className="flex-1"
                        onClick={handleDownload}
                        disabled={codes.length === 0}
                    >
                        {hasDownloaded ? <Check className="mr-2 h-4 w-4 text-green-500"/> :
                            <Download className="mr-2 h-4 w-4"/>}
                        Download .txt
                    </Button>
                </div>

                <AlertDialogFooter>
                    <Button
                        onClick={handleComplete}
                        disabled={(!hasCopied && !hasDownloaded) || codes.length === 0}
                        className="w-full"
                    >
                        I have securely saved this code
                    </Button>
                </AlertDialogFooter>
            </AlertDialogContent>
        </AlertDialog>
    );
}