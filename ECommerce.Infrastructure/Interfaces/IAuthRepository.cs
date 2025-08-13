using ECommerce.Domain.Entities;

namespace ECommerce.Infrastructure.Interfaces;

public interface IAuthRepository
{
    // User Registration
    Task<User> RegisterAsync(User user);

    // User Authentication
    Task<User> GetUserByEmailAsync(string email);
    Task<bool> UserExistsAsync(string email);

    // Password Management
    Task UpdateUserPasswordAsync(int userId, string newPasswordHash);

    // User Management
    Task<User> GetUserByIdAsync(int userId);
    Task UpdateUserAsync(User user);
}