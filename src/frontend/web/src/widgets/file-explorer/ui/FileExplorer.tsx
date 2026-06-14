import {
    Archive,
    ArrowLeft,
    ChevronRight,
    Code,
    File as FileIcon,
    FileText,
    Home,
    Image as ImageIcon,
    Music,
    Video,
    Folder,
    Download
} from "lucide-react";
import {useNavigate} from "react-router-dom";
import {useEffect, useState} from "react";
import {backendBaseUrl} from "@/shared/config";
import {Button} from "@/shared/ui/button.tsx";
import {base64ToBytes, cn, formatBytes} from "@/shared/lib";
import {
    Table, TableBody, TableCell, TableHead, TableHeader, TableRow,
} from "@/shared/ui/table";
import { format } from "date-fns";
import { setGuest } from "@/entities/session";
import {
    ContextMenu,
    ContextMenuContent,
    ContextMenuItem,
    ContextMenuTrigger,
} from "@/shared/ui/context-menu";
import { toast } from "sonner";
import {
    type FileResponse,
    type FolderAncestryResponse,
    type FolderResponse,
    type ListContentResponse,
    vaultKeys
} from "@/entities/folder";
import {decryptBytes, decryptString, importKey} from "@/shared/security";

function getFileIcon(filename: string) {
    const ext = filename.split('.').pop()?.toLowerCase();
    switch (ext) {
        case 'jpg':
        case 'jpeg':
        case 'png':
        case 'gif':
            return <ImageIcon className="h-5 w-5 text-purple-500"/>;
        case 'pdf':
        case 'txt':
        case 'doc':
        case 'docx':
        case 'md':
            return <FileText className="h-5 w-5 text-orange-500"/>;
        case 'mp4':
        case 'mkv':
        case 'avi':
            return <Video className="h-5 w-5 text-red-500"/>;
        case 'mp3':
        case 'wav':
        case 'flac':
            return <Music className="h-5 w-5 text-yellow-500"/>;
        case 'js':
        case 'cs' :
        case 'py' :
        case 'java' :
        case 'cpp' :
            return <Code className="h-5 w-5 text-green-500"/>;
        case 'zip':
        case 'rar':
        case '7z' :
        case 'tar.gz' :
        case 'tar':
        case 'tar.bz2' :
            return <Archive className="h-5 w-5 text-stone-500"/>;
        default:
            return <FileIcon className="h-5 w-5 text-slate-500"/>;
    }
}

type SortKey = "name" | "size" | "updatedAt";
type SortDirection = "asc" | "desc";

interface FileExplorerProps {
    folderId: string;
    rootFolderId: string;
}

interface UIFile extends FileResponse {
    decryptedName: string;
}

interface UIFolder extends FolderResponse {
    decryptedName: string;
}

export function FileExplorer({ folderId, rootFolderId }: FileExplorerProps) {
    const navigate = useNavigate();
    const [data, setData] = useState<{ folders: UIFolder[], files: UIFile[] } | null>(null);
    const [isLoading, setIsLoading] = useState(true);
    const [sortConfig, setSortConfig] = useState<{ key: SortKey; direction: SortDirection }>({
        key: "name",
        direction: "asc"
    });

    const [breadcrumbHistory, setBreadcrumbHistory] = useState<{ id: string; name: string }[]>(
        () => {
            const saved = sessionStorage.getItem("breadcrumbHistory");
            return saved ? JSON.parse(saved) : [];
        }
    );

    useEffect(() => {
        if (!rootFolderId) return;
        sessionStorage.removeItem("breadcrumbHistory");
        setBreadcrumbHistory([{ id: rootFolderId, name: "Home" }]);
    }, [rootFolderId]);

    useEffect(() => {
        if (!rootFolderId) return;

        let newHistory = [...breadcrumbHistory];

        if (folderId === rootFolderId) {
            newHistory = [{id: rootFolderId, name: "Home"}];
        } else if (newHistory.length === 0 || !newHistory.find(b => b.id === folderId)) {
            newHistory = [{id: rootFolderId, name: "Home"}, {id: folderId, name: "Unknown Folder"}];
        }

        setBreadcrumbHistory(newHistory);
        sessionStorage.setItem("breadcrumbHistory", JSON.stringify(newHistory));
    }, [folderId, rootFolderId]);

    useEffect(() => {
        const fetchData = async () => {
            try {
                setIsLoading(true);

                let currentFolderKey = vaultKeys.getKey(folderId);
                if (!currentFolderKey && folderId !== rootFolderId) {
                    const ancestryResponse = await fetch(`${backendBaseUrl}/api/Folder/GetAncestry?folderId=${folderId}`, {
                        credentials: "include",
                        headers: { "Content-Type": "application/json" }
                    });

                    if (ancestryResponse.status === 401) {
                        setGuest();
                        navigate("/login", { replace: true });
                        return;
                    }

                    if (!ancestryResponse.ok) throw new Error("Could not fetch folder lineage.");

                    const ancestryData = await ancestryResponse.json() as FolderAncestryResponse;
                    const lineage = ancestryData.items;

                    const rootKey = vaultKeys.getKey(rootFolderId);
                    if (!rootKey) throw new Error("Root key missing. Cannot rebuild chain. Please log in again.");

                    for (let i = 1; i < lineage.length; i++) {
                        const parentId = lineage[i - 1].id;
                        const parentKey = vaultKeys.getKey(parentId);

                        if (!parentKey) break;

                        const keyBytes = base64ToBytes(lineage[i].encryptedKey);
                        const decryptedRawKey = await decryptBytes(parentKey, keyBytes);
                        const folderCryptoKey = await importKey(decryptedRawKey);

                        vaultKeys.setKey(lineage[i].id, folderCryptoKey);
                    }

                    currentFolderKey = vaultKeys.getKey(folderId);
                }

                if (!currentFolderKey) {
                    toast.error("Unable to verify security credentials for this directory.");
                    navigate(`/folder/${rootFolderId}`, { replace: true });
                    return;
                }

                const res = await fetch(`${backendBaseUrl}/api/Folder/GetContent?FolderId=${folderId}`, {
                    credentials: "include",
                    headers: {"Content-Type": "application/json"}
                });

                if (res.status === 401) {
                    setGuest();
                    navigate("/login", { replace: true });
                    return;
                }

                if (!res.ok) throw new Error("Failed to load content.");
                const json = (await res.json()) as ListContentResponse;

                const foldersWithNames: UIFolder[] = await Promise.all(
                    json.folders.map(async (f) => {
                        try {
                            const nameBytes = base64ToBytes(f.nameEncrypted);
                            const keyBytes = base64ToBytes(f.folderKeyEncrypted);

                            const decryptedName = await decryptString(currentFolderKey!, nameBytes);
                            const decryptedRawKey = await decryptBytes(currentFolderKey!, keyBytes);
                            const subFolderCryptoKey = await importKey(decryptedRawKey);

                            vaultKeys.setKey(f.folderId, subFolderCryptoKey);

                            return { ...f, decryptedName };
                        } catch (err) {
                            return { ...f, decryptedName: "Unable do decrypt" };
                        }
                    })
                );

                const filesWithNames: UIFile[] = await Promise.all(
                    json.files.map(async (f) => {
                        try {
                            const nameBytes = base64ToBytes(f.fileNameEncrypted);
                            const decryptedName = await decryptString(currentFolderKey!, nameBytes);
                            return { ...f, decryptedName };
                        } catch (err) {
                            return { ...f, decryptedName: "Unable do decrypt" };
                        }
                    })
                );

                setData({folders: foldersWithNames, files: filesWithNames});
            } catch (error) {
                console.error("Failed to fetch folder content", error);
            } finally {
                setIsLoading(false);
            }
        };

        fetchData();
    }, [folderId, navigate]);

    const handleSort = (key: SortKey) => {
        setSortConfig(current => ({
            key,
            direction: current.key === key && current.direction === "asc" ? "desc" : "asc"
        }));
    };

    const navigateToSubFolder = (id: string, name: string) => {
        let newHistory = [...breadcrumbHistory];
        const existingIdx = newHistory.findIndex(b => b.id === id);

        if (existingIdx !== -1) {
            newHistory = newHistory.slice(0, existingIdx + 1);
        } else {
            newHistory.push({id, name});
        }

        setBreadcrumbHistory(newHistory);
        sessionStorage.setItem("breadcrumbHistory", JSON.stringify(newHistory));
        navigate(`/folder/${id}`);
    };

    const goBack = () => {
        if (breadcrumbHistory.length > 1) {
            const newHistory = [...breadcrumbHistory];
            newHistory.pop();

            setBreadcrumbHistory(newHistory);
            sessionStorage.setItem("breadcrumbHistory", JSON.stringify(newHistory));

            const prev = newHistory[newHistory.length - 1];
            navigate(`/folder/${prev.id}`);
        } else {
            navigate(`/`);
        }
    };

    if (!rootFolderId) return null;
    if (isLoading) return <div className="p-8 text-center text-slate-500">Decrypting and loading files...</div>;
    if (!data) return null;

    const sortItems = <T extends UIFile | UIFolder>(items: T[]): T[] => {
        return [...items].sort((a, b) => {
            let valA, valB;
            if (sortConfig.key === "name") {
                valA = a.decryptedName.toLowerCase();
                valB = b.decryptedName.toLowerCase();
            } else if (sortConfig.key === "size") {
                valA = 'sizeBytes' in a ? a.sizeBytes : 0;
                valB = 'sizeBytes' in b ? b.sizeBytes : 0;
            } else {
                valA = new Date(a.updatedAt).getTime();
                valB = new Date(b.updatedAt).getTime();
            }

            if (valA < valB) return sortConfig.direction === "asc" ? -1 : 1;
            if (valA > valB) return sortConfig.direction === "asc" ? 1 : -1;
            return 0;
        });
    };

    const sortedFolders = sortItems(data.folders);
    const sortedFiles = sortItems(data.files);

    const SortableHeader = ({label, sortKey, className = ""}: {
        label: string,
        sortKey: SortKey,
        className?: string
    }) => {
        const isRight = className.includes("text-right");

        return (
            <TableHead className={className}>
                <div
                    className={cn(
                        "flex items-center gap-1 cursor-pointer select-none font-semibold hover:text-foreground transition-colors",
                        isRight && "justify-end"
                    )}
                    onClick={() => handleSort(sortKey)}
                >
                    {label} {sortConfig.key === sortKey && (sortConfig.direction === "asc" ? "↑" : "↓")}
                </div>
            </TableHead>
        );
    };

    const handleDownload = async (fileId: string, fileName: string) => {
        const loadingToast = toast.loading(`Downloading ${fileName}...`);

        try {
            const res = await fetch(`${backendBaseUrl}/api/File/${fileId}`, {
                method: "GET",
                credentials: "include"
            });

            if (!res.ok) throw new Error("Failed to download file.");

            const data = await res.json();

            const parentFolderKey = vaultKeys.getKey(folderId);
            if (!parentFolderKey) throw new Error("Cryptographic context missing. Please refresh.");

            const encryptedKeyBytes = base64ToBytes(data.encryptedKey);
            const rawFileKeyBytes = await decryptBytes(parentFolderKey, encryptedKeyBytes);
            const fileCryptoKey = await importKey(rawFileKeyBytes);

            const encryptedContentBytes = base64ToBytes(data.fileContent);
            const decryptedPlaintextBytes = await decryptBytes(fileCryptoKey, encryptedContentBytes);

            const blob = new Blob([decryptedPlaintextBytes as unknown as BufferSource], { type: data.mimeType || "application/octet-stream" });
            const url = window.URL.createObjectURL(blob);

            const a = document.createElement("a");
            a.href = url;
            a.download = fileName;
            document.body.appendChild(a);
            a.click();

            window.URL.revokeObjectURL(url);
            document.body.removeChild(a);

            toast.success("Download complete", { id: loadingToast });
        } catch (error) {
            console.error("Download error:", error);
            toast.error("Failed to download file", { id: loadingToast });
        }
    };

    return (
        <div>
            <div className="flex items-center gap-2 mb-4 bg-muted/30 p-2 rounded-md">
                <Button variant="ghost" size="icon" onClick={() => navigateToSubFolder(rootFolderId, "Home")}
                        title="Home">
                    <Home className="h-4 w-4"/>
                </Button>
                <Button variant="ghost" size="icon" onClick={goBack} disabled={folderId === rootFolderId} title="Back">
                    <ArrowLeft className="h-4 w-4"/>
                </Button>

                <div className="h-4 w-px bg-border mx-2"/>

                <div className="flex items-center flex-wrap gap-1 text-sm font-medium">
                    {breadcrumbHistory.map((crumb, idx) => (
                        <div key={crumb.id} className="flex items-center">
                            <button
                                className={cn("hover:text-primary transition-colors cursor-pointer", idx === breadcrumbHistory.length - 1 ? "text-primary" : "text-muted-foreground")}
                                onClick={() => navigateToSubFolder(crumb.id, crumb.name)}
                            >
                                {crumb.name}
                            </button>
                            {idx < breadcrumbHistory.length - 1 && (
                                <ChevronRight className="h-4 w-4 mx-1 text-muted-foreground"/>
                            )}
                        </div>
                    ))}
                </div>
            </div>

            <div className="rounded-md border bg-card text-card-foreground shadow-sm">
                <Table>
                    <TableHeader>
                        <TableRow>
                            <SortableHeader label="Name" sortKey="name" className="w-full"/>
                            <SortableHeader label="Modified" sortKey="updatedAt" className="min-w-37.5 w-37.5"/>
                            <SortableHeader label="Size" sortKey="size"
                                            className="min-w-25 w-25 whitespace-nowrap text-right"/>
                        </TableRow>
                    </TableHeader>
                    <TableBody>
                        {sortedFolders.map((folder) => (
                            <TableRow
                                key={folder.folderId}
                                className="cursor-pointer hover:bg-muted/50"
                                onClick={() => navigateToSubFolder(folder.folderId, folder.decryptedName)}
                            >
                                <TableCell className="font-medium flex items-center gap-3">
                                    <Folder className="h-5 w-5 text-blue-500 fill-blue-500/20"/>
                                    <span className="truncate max-w-100"
                                          title={folder.decryptedName}>{folder.decryptedName}</span>
                                </TableCell>
                                <TableCell className="text-muted-foreground whitespace-nowrap">
                                    {format(new Date(folder.updatedAt), "dd-MM-yyyy HH:mm")}
                                </TableCell>
                                <TableCell className="text-muted-foreground text-right">--</TableCell>
                            </TableRow>
                        ))}

                        {sortedFiles.map((file) => (
                            <ContextMenu key={file.filesId}>
                                <ContextMenuTrigger
                                    render={
                                        <TableRow
                                            className="hover:bg-muted/50 cursor-context-menu"
                                            style={{ display: "table-row" }}
                                        />
                                    }
                                >
                                    <TableCell className="font-medium flex items-center gap-3">
                                        {getFileIcon(file.decryptedName)}
                                        <span className="truncate max-w-100"
                                              title={file.decryptedName}>{file.decryptedName}</span>
                                    </TableCell>
                                    <TableCell className="text-muted-foreground whitespace-nowrap">
                                        {format(new Date(file.updatedAt), "dd-MM-yyyy HH:mm")}
                                    </TableCell>
                                    <TableCell className="text-muted-foreground whitespace-nowrap text-right">
                                        {formatBytes(file.sizeBytes)}
                                    </TableCell>
                                </ContextMenuTrigger>
                                <ContextMenuContent className="w-48">
                                    <ContextMenuItem onClick={() => handleDownload(file.filesId, file.decryptedName)}>
                                        <Download className="mr-2 h-2 w-2" />
                                        Download
                                    </ContextMenuItem>
                                </ContextMenuContent>
                            </ContextMenu>
                        ))}

                        {sortedFolders.length === 0 && sortedFiles.length === 0 && (
                            <TableRow>
                                <TableCell colSpan={3} className="h-32 text-center text-muted-foreground">
                                    This folder is empty.
                                </TableCell>
                            </TableRow>
                        )}
                    </TableBody>
                </Table>
            </div>
        </div>
    )
}