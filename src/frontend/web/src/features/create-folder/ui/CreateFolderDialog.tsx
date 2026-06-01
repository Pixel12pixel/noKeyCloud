import {useState} from "react";
import {Button} from "@/shared/ui/button";
import {Input} from "@/shared/ui/input";
import {Label} from "@/shared/ui/label";
import {toast} from "sonner";
import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogFooter,
    DialogHeader,
    DialogTitle,
    DialogTrigger,
} from "@/shared/ui/dialog";
import {backendBaseUrl} from "@/shared/config";

async function encryptName(name: string): Promise<string> {
    // TODO: implement name encryption
    return name;
}

interface CreateFolderDialogProps {
    parentId: string;
    children: React.ReactNode;
    onSuccess?: () => void;
}

export function CreateFolderDialog({parentId, children, onSuccess}: CreateFolderDialogProps) {
    const [open, setOpen] = useState(false);
    const [folderName, setFolderName] = useState("");
    const [isSubmitting, setIsSubmitting] = useState(false);

    const handleSubmit = async (e: React.SyntheticEvent) => {
        e.preventDefault();
        if (!folderName.trim()) return;

        try {
            setIsSubmitting(true);
            const encryptedName = await encryptName(folderName);

            const payload = {
                name: encryptedName,
                parentFolderId: parentId
            };

            const response = await fetch(`${backendBaseUrl}/api/Folder`, {
                method: "POST",
                credentials: "include",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(payload)
            });

            if (!response.ok) throw new Error("Failed to create folder");

            setFolderName("");
            setOpen(false);

            if (onSuccess) onSuccess();

            toast.success("Folder created successfully", {
                description: `"${folderName}" was created successfully.`
            });

        } catch (error) {
            console.error(error);
            toast.error("Failed to create folder", {
                description: "Please check your connection and try again."
            });
        } finally {
            setIsSubmitting(false);
        }
    };

    return (
        <Dialog open={open} onOpenChange={setOpen}>
            <DialogTrigger render={children as React.ReactElement}/>

            <DialogContent className="sm:max-w-[425px]">
                <form onSubmit={handleSubmit}>
                    <DialogHeader>
                        <DialogTitle>Create New Folder</DialogTitle>
                        <DialogDescription>
                            Enter a name for your new folder.
                        </DialogDescription>
                    </DialogHeader>

                    <div className="grid gap-4 py-4">
                        <div className="grid gap-2">
                            <Label htmlFor="name">Folder Name</Label>
                            <Input
                                id="name"
                                value={folderName}
                                onChange={(e) => setFolderName(e.target.value)}
                                placeholder="e.g. Unpaid Taxes"
                                autoFocus
                                disabled={isSubmitting}
                            />
                        </div>
                    </div>

                    <DialogFooter>
                        <Button
                            type="button"
                            variant="outline"
                            onClick={() => setOpen(false)}
                            disabled={isSubmitting}
                        >
                            Cancel
                        </Button>
                        <Button type="submit" disabled={isSubmitting || !folderName.trim()}>
                            {isSubmitting ? "Creating..." : "Create Folder"}
                        </Button>
                    </DialogFooter>
                </form>
            </DialogContent>
        </Dialog>
    );
}