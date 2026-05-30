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
    Folder
} from "lucide-react";
import {useNavigate} from "react-router-dom";
import {useEffect, useState} from "react";
import {backendBaseUrl} from "@/shared/config";
import {Button} from "@/shared/ui/button.tsx";
import {cn} from "@/shared/lib/utils.ts";
import {
    Table, TableBody, TableCell, TableHead, TableHeader, TableRow,
} from "@/shared/ui/table";
import { format } from "date-fns";
import { setGuest } from "@/entities/session/model/authStore";

async function decryptName(encryptedBase64: string): Promise<string> {
    try {
        // TODO: Replace with actual name decryption when encryption implemented
        const binString = atob(encryptedBase64);
        const bytes = Uint8Array.from(binString, (m) => m.codePointAt(0)!);
        return new TextDecoder().decode(bytes);
    } catch {
        return "Unknown File";
    }
}

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

const formatBytes = (bytes: number, decimals = 2) => {
    if (!+bytes) return '0 Bytes'
    const k = 1024
    const dm = decimals < 0 ? 0 : decimals
    const sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB']
    const i = Math.floor(Math.log(bytes) / Math.log(k))
    return `${parseFloat((bytes / Math.pow(k, i)).toFixed(dm))} ${sizes[i]}`
}

type SortKey = "name" | "size" | "updatedAt";
type SortDirection = "asc" | "desc";

interface FileExplorerProps {
    folderId: string;
    rootFolderId: string;
}

export function FileExplorer({ folderId, rootFolderId }: FileExplorerProps) {
    const navigate = useNavigate();
    const [data, setData] = useState<{ folders: any[], files: any[] } | null>(null);
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
                const json = await res.json();

                const foldersWithNames = await Promise.all(json.folders.map(async (f: any) => ({
                    ...f,
                    decryptedName: await decryptName(f.nameEncrypted),
                })));

                const filesWithNames = await Promise.all(json.files.map(async (f: any) => ({
                    ...f,
                    decryptedName: await decryptName(f.fileNameEncrypted),
                })));

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

    const sortItems = (items: any[]) => {
        return [...items].sort((a, b) => {
            let valA, valB;
            if (sortConfig.key === "name") {
                valA = a.decryptedName.toLowerCase();
                valB = b.decryptedName.toLowerCase();
            } else if (sortConfig.key === "size") {
                valA = a.sizeBytes || 0;
                valB = b.sizeBytes || 0;
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

                <div className="h-4 w-[1px] bg-border mx-2"/>

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
                            <SortableHeader label="Name" sortKey="name" className="w-[100%]"/>
                            <SortableHeader label="Modified" sortKey="updatedAt" className="min-w-[150px] w-[150px]"/>
                            <SortableHeader label="Size" sortKey="size"
                                            className="min-w-[100px] w-[100px] whitespace-nowrap text-right"/>
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
                                    <span className="truncate max-w-[400px]"
                                          title={folder.decryptedName}>{folder.decryptedName}</span>
                                </TableCell>
                                <TableCell className="text-muted-foreground whitespace-nowrap">
                                    {format(new Date(folder.updatedAt), "dd-MM-yyyy HH:mm")}
                                </TableCell>
                                <TableCell className="text-muted-foreground text-right">--</TableCell>
                            </TableRow>
                        ))}

                        {sortedFiles.map((file) => (
                            <TableRow key={file.filesId} className="hover:bg-muted/50">
                                <TableCell className="font-medium flex items-center gap-3">
                                    {getFileIcon(file.decryptedName)}
                                    <span className="truncate max-w-[400px]"
                                          title={file.decryptedName}>{file.decryptedName}</span>
                                </TableCell>
                                <TableCell className="text-muted-foreground whitespace-nowrap">
                                    {format(new Date(file.updatedAt), "dd-MM-yyyy HH:mm")}
                                </TableCell>
                                <TableCell className="text-muted-foreground whitespace-nowrap text-right">
                                    {formatBytes(file.sizeBytes)}
                                </TableCell>
                            </TableRow>
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