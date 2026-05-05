using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace noKeyCloud.Application.Abstractions.Services
{
    public interface IJwtService
    {
        Task<string> JwtTokenService(Guid Id);
    }
}
