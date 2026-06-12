using MediatR;
using noKeyCloud.Contracts.Common;

namespace noKeyCloud.Application.Features.Users.Register;

public record RegisterUserCommand(
    string Username,
    string Email,
    byte[] Salt,
    byte[] Verifier,
    byte[] EncryptedMasterKey,
    byte[] KeySalt,
    byte[] RecoveryEncryptedMasterKey,
    byte[] RootFolderKey,
    string? RegisterInviteCode) : IRequest<Result>;