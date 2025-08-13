using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Data;
using ECommerce.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories;

public class UserInRoleRepository: IUserInRoleRepository
{
     
   
    private readonly ECommerceDbContext _dbContext;
    public UserInRoleRepository(ECommerceDbContext dbContext) => _dbContext = dbContext;

    public Task<UserInRole?> GetByIdAsync(int id, CancellationToken ct = default) =>
         _dbContext.UserInRole.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == id);

    public async Task AddAsync(UserInRole userInRole)
    {
        await _dbContext.UserInRole.AddAsync(userInRole);
        await _dbContext.SaveChangesAsync();
    }

}
