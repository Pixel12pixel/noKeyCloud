const keyCache = new Map<string, CryptoKey>();

export const vaultKeys = {
    setKey: (folderId: string, key: CryptoKey) => {
        keyCache.set(folderId, key);
    },
    getKey: (folderId: string): CryptoKey | undefined => {
        return keyCache.get(folderId);
    },
    clear: () => {
        keyCache.clear();
    }
};