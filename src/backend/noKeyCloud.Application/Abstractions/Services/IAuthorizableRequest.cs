using System;
using System.Collections.Generic;
using System.Text;

namespace noKeyCloud.Application.Abstractions.Services
{
    public interface IAuthorizableRequest
    {
        Guid Id { get; }
    }
}
