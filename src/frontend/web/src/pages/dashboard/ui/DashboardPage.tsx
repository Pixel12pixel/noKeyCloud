import { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useAuth } from "@/entities/session/model/useAuth";
import { FileExplorer } from "@/widgets/file-explorer/ui/FileExplorer";
import { Button } from "@/shared/ui/button";
import { FolderPlus } from "lucide-react";
import { CreateFolderDialog } from "@/features/create-folder/ui/CreateFolderDialog";

export function DashboardPage() {
    const navigate = useNavigate();
    const { folderId } = useParams();
    const auth = useAuth();
    const [refreshKey, setRefreshKey] = useState(0);

    useEffect(() => {
        document.title = "noKeyCloud";
    }, []);

    useEffect(() => {
        if (auth.status === "guest") {
            navigate("/login", { replace: true });
        }
    }, [auth.status, navigate]);

    const rootFolderId = auth.rootFolderId ?? "";
    const currentFolderId = folderId || rootFolderId;

    if (auth.status !== "authenticated" || !currentFolderId) {
        return null;
    }

    return (
        <div className="flex-1 p-8 pt-4">
            <div className="flex flex-col sm:flex-row items-center justify-between mb-4">
                <div className="flex items-center gap-2">
                    <CreateFolderDialog
                        parentId={currentFolderId}
                        onSuccess={() => setRefreshKey((prev) => prev + 1)}
                    >
                        <Button>
                            <FolderPlus className="h-4 w-4 mr-2" />
                            New Folder
                        </Button>
                    </CreateFolderDialog>
                    <Button variant="secondary">
                        Upload File
                    </Button>
                </div>
            </div>

            <FileExplorer
                key={refreshKey}
                folderId={currentFolderId}
                rootFolderId={rootFolderId}
            />
        </div>
    );
}