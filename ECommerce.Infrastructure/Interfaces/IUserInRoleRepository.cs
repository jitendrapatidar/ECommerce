using ECommerce.Domain.Entities;

namespace ECommerce.Infrastructure.Interfaces;

public interface IUserInRoleRepository
{
    Task<UserInRole?> GetByIdAsync(int id, CancellationToken ct = default);

    Task AddAsync(UserInRole userInRole);
}