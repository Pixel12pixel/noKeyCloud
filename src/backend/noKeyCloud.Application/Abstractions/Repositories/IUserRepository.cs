
using noKeyCloud.Domain.Entities;

namespace noKeyCloud.Application.Abstractions.Repositories;

public interface IUserRepository
{
    Task CreateUser(User user);
}