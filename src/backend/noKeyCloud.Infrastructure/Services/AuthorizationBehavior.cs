using MediatR;
using Microsoft.AspNetCore.Authorization;
using noKeyCloud.Application.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace noKeyCloud.Infrastructure.Services;

public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IAuthorizableRequest
{
    private readonly IAuthorizationService _authorizationService;
    private readonly ICurrentUserSerivce _currentUserService;

    public AuthorizationBehavior(IAuthorizationService authorizationService, ICurrentUserSerivce currentUserService)
    {
        _authorizationService = authorizationService;
        _currentUserService = currentUserService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var user = _currentUserService.User;
        if (user == null)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }
        var authorizationResult = await _authorizationService.AuthorizeAsync(user, request.Id, "DefaultPolicy");
        if (!authorizationResult.Succeeded)
        {
            throw new UnauthorizedAccessException("User is not authorized to perform this action.");
        }
        return await next();
    }


}

