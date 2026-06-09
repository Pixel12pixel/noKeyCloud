export interface RegisterInviteResponse {
    id: string;
    code: string;
    createdAt: string;
    expiresAt: string | null;
}