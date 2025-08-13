using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Data;
using ECommerce.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly ECommerceDbContext _dbContext;
    public RoleRepository(ECommerceDbContext dbContext) => _dbContext = dbContext;

    public Task<Role?> GetByRoleNameAsync(string rolename) =>
        _dbContext.Set<Role>().AsNoTracking().FirstOrDefaultAsync(u => u.RoleName == rolename);
    public Task<Role?> GetByIdAsync(int id, CancellationToken ct = default) =>
         _dbContext.Set<Role>().AsNoTracking().FirstOrDefaultAsync(u => u.RoleId == id);

}
 