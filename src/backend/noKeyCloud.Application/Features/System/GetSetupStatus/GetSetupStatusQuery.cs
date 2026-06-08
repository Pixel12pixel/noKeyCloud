using MediatR;

namespace noKeyCloud.Application.Features.System.GetSetupStatus;

public record GetSetupStatusQuery : IRequest<bool>;