export interface FileResponse {
    filesId: string;
    fileNameEncrypted: string;
    mimeType: string;
    sizeBytes: number;
    fileKeyEncrypted: string;
    checkSum: string;
    createdAt: string;
    updatedAt: string;
}

export interface FolderResponse {
    folderId: string;
    nameEncrypted: string;
    folderKeyEncrypted: string;
    createdAt: string;
    updatedAt: string;
    parentFolderId: string | null;
}

export interface ListContentResponse {
    files: FileResponse[];
    folders: FolderResponse[];
}