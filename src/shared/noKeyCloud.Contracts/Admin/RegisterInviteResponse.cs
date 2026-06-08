namespace noKeyCloud.Contracts.Admin;

public record RegisterInviteResponse(Guid Id, string Code, DateTime CreatedAt, DateTime? ExpiresAt);