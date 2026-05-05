using Microsoft.AspNetCore.Http;
using noKeyCloud.Application.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace noKeyCloud.Infrastructure.Services
{
    public sealed class CurrentUserService : ICurrentUserSerivce
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public CurrentUserService(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public ClaimsPrincipal User
        {
            get
            {
                var user = _contextAccessor.HttpContext?.User;

                if (user == null)
                {
                    throw new InvalidOperationException("No user context available.");
                }
                return user;
            }   
        }
    }
}
