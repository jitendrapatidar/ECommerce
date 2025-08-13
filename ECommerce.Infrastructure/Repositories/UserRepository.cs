using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Data;
using ECommerce.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace ECommerce.Infrastructure.Repositories;

public class UserRepository: IUserRepository
{
    private readonly ECommerceDbContext _dbContext;
    public UserRepository(ECommerceDbContext dbContext) => _dbContext = dbContext;

    public Task<User?> GetByUsernameAsync(string username) =>
        _dbContext.Set<User>().AsNoTracking().FirstOrDefaultAsync(u => u.Username == username);
    public Task<User?> GetByIdAsync(int id, CancellationToken ct = default) =>
         _dbContext.Set<User>().AsNoTracking().FirstOrDefaultAsync(u => u.UserId == id);

    public Task<IEnumerable<User>> GetAllAsync(CancellationToken ct = default) =>
      _dbContext.Set<User>().AsNoTracking()
      .ToListAsync(ct).ContinueWith(t => (IEnumerable<User>)t.Result);
    public async Task<User?> UserLoginAsync(string username, string password)
    {
        // 1. Hash incoming password
        string hashedPassword = ComputeSha256Hash(password);

        // 2. Query user with hashed password
        return await _dbContext.Set<User>()
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == username && u.PasswordHash == hashedPassword);
    }
    public async Task AddAsync(User user)
    {
        await _dbContext.Set<User>().AddAsync(user);
        await _dbContext.SaveChangesAsync();
    }
    public async Task UpdateAsync(User user, CancellationToken ct = default)
    {
        var existingCategory = await GetByIdAsync(user.UserId);
        if (existingCategory == null) throw new KeyNotFoundException("Category not found");
        _dbContext.Set<User>().Update(user);
        await _dbContext.SaveChangesAsync(ct);
        
    }
    public async Task DeleteAsync(User user, CancellationToken ct = default)
    {
        _dbContext.Set<User>().Remove(user);
        await _dbContext.SaveChangesAsync(ct);
    }

    private string ComputeSha256Hash(string rawData)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2")); // hex format
            }
            return builder.ToString();
        }
    }
}
