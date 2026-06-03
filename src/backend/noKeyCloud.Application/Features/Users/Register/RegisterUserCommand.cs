using MediatR;
using noKeyCloud.Contracts.Common;

namespace noKeyCloud.Application.Features.Users.Register;

public record RegisterUserCommand(
    string Username,
    string Email,
    byte[] Salt,
    byte[] Verifier,
    string? RegisterInviteCode) : IRequest<Result>;