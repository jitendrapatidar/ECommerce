using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Interfaces;

public interface IUserRepository
{
 Task<User?> GetByUsernameAsync(string username);
 Task<User?> UserLoginAsync(string username, string password);
  Task AddAsync(User user);
  Task<User?> GetByIdAsync(int id, CancellationToken ct = default);

    Task<IEnumerable<User>> GetAllAsync(CancellationToken ct = default);
    Task UpdateAsync(User user, CancellationToken ct = default);
    Task DeleteAsync(User user, CancellationToken ct = default);
}