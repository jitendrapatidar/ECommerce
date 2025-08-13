using ECommerce.Domain.Entities;

namespace ECommerce.Infrastructure.Interfaces;

public interface IRoleRepository
{
    Task<Role?> GetByRoleNameAsync(string rolename);
    Task<Role?> GetByIdAsync(int id, CancellationToken ct = default);

}