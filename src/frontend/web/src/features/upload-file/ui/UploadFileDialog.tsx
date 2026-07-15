import { useState, useRef } from "react";
import { Button } from "@/shared/ui/button";
import { UploadCloud, Loader2, File as FileIcon, X, CheckCircle2, AlertCircle, Maximize2 } from "lucide-react";
import { toast } from "sonner";
import { backendBaseUrl } from "@/shared/config";
import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogFooter,
    DialogHeader,
    DialogTitle,
    DialogTrigger,
} from "@/shared/ui/dialog";
import {bytesToBase64, cn, formatBytes} from "@/shared/lib";
import {vaultKeys} from "@/entities/folder";
import {encryptBytes, encryptString, exportKey, generateAesKey} from "@/shared/security";
import {customFetch} from "@/shared/api";

interface UploadFileDialogProps {
    parentId: string;
    children: React.ReactNode;
    onSuccess?: () => void;
}

type FileStatus = 'pending' | 'processing' | 'uploading' | 'success' | 'error';

interface UploadFileState {
    id: string;
    file: File;
    status: FileStatus;
    errorMessage?: string;
}

export function UploadFileDialog({ parentId, children, onSuccess }: UploadFileDialogProps) {
    const [open, setOpen] = useState(false);
    const [isMinimized, setIsMinimized] = useState(false);
    const [isDragging, setIsDragging] = useState(false);
    const [files, setFiles] = useState<UploadFileState[]>([]);
    const [isUploading, setIsUploading] = useState(false);
    const fileInputRef = useRef<HTMLInputElement>(null);

    const handleDragOver = (e: React.DragEvent<HTMLDivElement>) => {
        e.preventDefault();
        e.stopPropagation();
        setIsDragging(true);
    };

    const handleDragLeave = (e: React.DragEvent<HTMLDivElement>) => {
        e.preventDefault();
        e.stopPropagation();
        setIsDragging(false);
    };

    const handleDrop = (e: React.DragEvent<HTMLDivElement>) => {
        e.preventDefault();
        e.stopPropagation();
        setIsDragging(false);

        if (e.dataTransfer.files && e.dataTransfer.files.length > 0) {
            addFiles(e.dataTransfer.files);
            e.dataTransfer.clearData();
        }
    };

    const handleFileSelect = (e: React.ChangeEvent<HTMLInputElement>) => {
        if (e.target.files && e.target.files.length > 0) {
            addFiles(e.target.files);
        }
        if (fileInputRef.current) fileInputRef.current.value = "";
    };

    const addFiles = (newFiles: FileList | File[]) => {
        const fileStates: UploadFileState[] = Array.from(newFiles).map(file => ({
            id: Math.random().toString(36).substring(7),
            file,
            status: 'pending'
        }));
        setFiles(prev => [...prev, ...fileStates]);
    };

    const removeFile = (id: string) => {
        setFiles(prev => prev.filter(f => f.id !== id));
    };

    const handleUpload = async () => {
        if (files.length === 0) return;

        setIsUploading(true);
        let hasError = false;

        const parentKey = vaultKeys.getKey(parentId);
        if (!parentKey) {
            toast.error("Vault is locked or parent key is missing.");
            setIsUploading(false);
            return;
        }

        for (let i = 0; i < files.length; i++) {
            const currentFile = files[i];

            if (currentFile.status === 'success') continue;

            try {
                setFiles(prev => prev.map(f => f.id === currentFile.id ? { ...f, status: 'processing' } : f));

                const arrayBuffer = await currentFile.file.arrayBuffer();
                const fileBytes = new Uint8Array(arrayBuffer);

                const fileKey = await generateAesKey();

                const encryptedFileBytes = await encryptBytes(fileKey, fileBytes);

                const exportedFileKey = await exportKey(fileKey);
                const encryptedFileKey = await encryptBytes(parentKey, exportedFileKey);

                const encryptedName = await encryptString(parentKey, currentFile.file.name);

                const checksumBuffer = await crypto.subtle.digest("SHA-256", encryptedFileBytes as unknown as BufferSource);
                const checksumBytes = new Uint8Array(checksumBuffer);

                const payload = {
                    fileName: bytesToBase64(encryptedName),
                    mimeType: currentFile.file.type || "application/octet-stream",
                    sizeBytes: encryptedFileBytes.length,
                    encryptedKey: bytesToBase64(encryptedFileKey),
                    checksum: bytesToBase64(checksumBytes),
                    folderId: parentId,
                    fileContent: bytesToBase64(encryptedFileBytes)
                };

                setFiles(prev => prev.map(f => f.id === currentFile.id ? { ...f, status: 'uploading' } : f));

                const response = await customFetch(`${backendBaseUrl}/api/File/upload`, {
                    method: "POST",
                    credentials: "include",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(payload)
                });

                if (!response.ok) throw new Error("Upload failed");

                setFiles(prev => prev.map(f => f.id === currentFile.id ? { ...f, status: 'success' } : f));

            } catch (error) {
                console.error(error);
                hasError = true;
                setFiles(prev => prev.map(f => f.id === currentFile.id ? { ...f, status: 'error', errorMessage: 'Failed' } : f));
            }
        }

        setIsUploading(false);

        if (!hasError) {
            toast.success("All files uploaded successfully");
            setTimeout(() => {
                setOpen(false);
                setIsMinimized(false);
                setFiles([]);
            }, 1000);
        } else {
            toast.error("Some files failed to upload.");
            setIsMinimized(false);
            setOpen(true);
        }

        if (onSuccess) onSuccess();
    };

    const handleOpenChange = (newOpen: boolean) => {
        if (isUploading && !newOpen) {
            setIsMinimized(true);
            setOpen(false);
            return;
        }

        if (!isUploading) {
            setOpen(newOpen);
            if (!newOpen) {
                setTimeout(() => setFiles([]), 200);
            }
        }
    };

    const completedCount = files.filter(f => f.status === 'success').length;
    const progressPercentage = files.length === 0 ? 0 : Math.round((completedCount / files.length) * 100);

    return (
        <>
            <Dialog open={open} onOpenChange={handleOpenChange}>
                <DialogTrigger render={children as React.ReactElement}/>

                <DialogContent className="sm:max-w-xl" showCloseButton={!isUploading}>
                    <DialogHeader>
                        <DialogTitle>Upload Files</DialogTitle>
                        <DialogDescription>
                            Drag and drop files here or click to browse.
                        </DialogDescription>
                    </DialogHeader>

                    <div className="py-2 flex flex-col gap-4 min-w-0">

                        <div
                            className={cn(
                                "flex flex-col items-center justify-center p-6 border-2 border-dashed rounded-lg transition-colors cursor-pointer bg-muted/30 w-full min-w-0",
                                isDragging ? "border-primary bg-primary/10" : "border-border hover:bg-muted/50 hover:border-primary/50"
                            )}
                            onDragOver={handleDragOver}
                            onDragLeave={handleDragLeave}
                            onDrop={handleDrop}
                            onClick={() => fileInputRef.current?.click()}
                        >
                            <UploadCloud className={cn("h-8 w-8 mb-3 transition-colors", isDragging ? "text-primary" : "text-muted-foreground")} />
                            <p className="text-sm font-medium mb-1 truncate max-w-full px-4">Click or drag files to this area</p>
                            <p className="text-xs text-muted-foreground text-center truncate max-w-full px-4">
                                Files will be encrypted locally before being stored securely.
                            </p>
                            <input
                                type="file"
                                multiple
                                className="hidden"
                                ref={fileInputRef}
                                onChange={handleFileSelect}
                            />
                        </div>

                        {files.length > 0 && (
                            <div className="flex flex-col gap-2 min-w-0">
                                <div className="flex items-center justify-between text-xs font-medium text-muted-foreground mb-1">
                                    <span>{completedCount} of {files.length} uploaded</span>
                                    <span>{progressPercentage}%</span>
                                </div>
                                <div className="h-2 w-full bg-muted rounded-full overflow-hidden mb-2 shrink-0">
                                    <div
                                        className="h-full bg-primary transition-all duration-300 ease-in-out"
                                        style={{ width: `${progressPercentage}%` }}
                                    />
                                </div>

                                <div className="max-h-[35vh] overflow-y-auto space-y-2 pr-1 custom-scrollbar min-w-0">
                                    {files.map((fileState) => (
                                        <div key={fileState.id} className="flex items-center justify-between p-3 border rounded-lg bg-card shadow-sm gap-3 min-w-0">
                                            <div className="p-2 bg-primary/10 rounded-md text-primary shrink-0">
                                                <FileIcon className="h-5 w-5" />
                                            </div>

                                            <div className="flex flex-col flex-1 min-w-0 overflow-hidden">
                                                <span className="text-sm font-medium truncate" title={fileState.file.name}>
                                                    {fileState.file.name}
                                                </span>
                                                <div className="flex items-center gap-2 text-xs text-muted-foreground mt-0.5 truncate">
                                                    <span className="shrink-0">{formatBytes(fileState.file.size)}</span>
                                                    {fileState.status === 'processing' && <span className="text-primary animate-pulse shrink-0">• Encrypting</span>}
                                                    {fileState.status === 'uploading' && <span className="text-blue-500 animate-pulse shrink-0">• Uploading</span>}
                                                    {fileState.status === 'error' && <span className="text-destructive truncate">• {fileState.errorMessage}</span>}
                                                </div>
                                            </div>

                                            <div className="shrink-0 flex items-center justify-center w-8">
                                                {fileState.status === 'success' ? (
                                                    <CheckCircle2 className="h-5 w-5 text-green-500" />
                                                ) : fileState.status === 'error' ? (
                                                    <AlertCircle className="h-5 w-5 text-destructive" />
                                                ) : (fileState.status === 'processing' || fileState.status === 'uploading') ? (
                                                    <Loader2 className="h-4 w-4 animate-spin text-primary" />
                                                ) : (
                                                    <Button
                                                        variant="ghost"
                                                        size="icon"
                                                        className="h-8 w-8 text-muted-foreground hover:text-destructive"
                                                        onClick={() => removeFile(fileState.id)}
                                                        disabled={isUploading}
                                                    >
                                                        <X className="h-4 w-4" />
                                                    </Button>
                                                )}
                                            </div>
                                        </div>
                                    ))}
                                </div>
                            </div>
                        )}
                    </div>

                    <DialogFooter>
                        <Button
                            type="button"
                            variant="outline"
                            onClick={() => handleOpenChange(false)}
                        >
                            {isUploading ? "Minimize" : "Cancel"}
                        </Button>
                        <Button
                            onClick={handleUpload}
                            disabled={files.length === 0 || isUploading || files.every(f => f.status === 'success')}
                        >
                            {isUploading && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
                            {isUploading ? "Processing..." : "Upload Files"}
                        </Button>
                    </DialogFooter>
                </DialogContent>
            </Dialog>

            {isMinimized && isUploading && (
                <div
                    className="fixed bottom-4 right-4 z-50 w-80 bg-card border rounded-lg shadow-lg p-4 cursor-pointer hover:border-primary/50 transition-all animate-in slide-in-from-bottom-5 fade-in"
                    onClick={() => {
                        setIsMinimized(false);
                        setOpen(true);
                    }}
                >
                    <div className="flex items-center justify-between mb-3">
                        <div className="flex items-center gap-2 font-medium">
                            <Loader2 className="h-4 w-4 animate-spin text-primary" />
                            <span className="text-sm">Uploading {files.length} files...</span>
                        </div>
                        <Button variant="ghost" size="icon" className="h-6 w-6 rounded-full shrink-0">
                            <Maximize2 className="h-3 w-3" />
                        </Button>
                    </div>

                    <div className="flex items-center justify-between text-xs font-medium text-muted-foreground mb-1">
                        <span>{completedCount} of {files.length} completed</span>
                        <span>{progressPercentage}%</span>
                    </div>
                    <div className="h-1.5 w-full bg-muted rounded-full overflow-hidden">
                        <div
                            className="h-full bg-primary transition-all duration-300 ease-in-out"
                            style={{ width: `${progressPercentage}%` }}
                        />
                    </div>
                </div>
            )}
        </>
    );
}