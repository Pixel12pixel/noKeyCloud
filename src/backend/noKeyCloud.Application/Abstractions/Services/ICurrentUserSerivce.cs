using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace noKeyCloud.Application.Abstractions.Services
{
    public interface ICurrentUserSerivce
    {
        ClaimsPrincipal User { get; }
    }
}
