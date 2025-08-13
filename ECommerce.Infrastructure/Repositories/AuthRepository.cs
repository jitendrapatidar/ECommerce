using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Data;
using ECommerce.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Repositories;

public class AuthRepository: IAuthRepository
{
    private readonly ECommerceDbContext _context;

    public AuthRepository(ECommerceDbContext context)
    {
        _context = context;
    }

    // User Registration
    public async Task<User> RegisterAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }

    // User Authentication
    public async Task<User> GetUserByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> UserExistsAsync(string email)
    {
        return await _context.Users
            .AnyAsync(u => u.Email == email);
    }

    // Password Management
    public async Task UpdateUserPasswordAsync(int userId, string newPasswordHash)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.PasswordHash = newPasswordHash;
            await _context.SaveChangesAsync();
        }
    }

    // User Management
    public async Task<User> GetUserByIdAsync(int userId)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    //// Refresh Token Management (Optional)
    //public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
    //{
    //    await _context.RefreshTokens.AddAsync(refreshToken);
    //    await _context.SaveChangesAsync();
    //}

    //public async Task<RefreshToken> GetRefreshTokenAsync(string token)
    //{
    //    return await _context.RefreshTokens
    //        .Include(rt => rt.User)
    //        .FirstOrDefaultAsync(rt => rt.Token == token);
    //}

    //public async Task RevokeRefreshTokenAsync(string token)
    //{
    //    var refreshToken = await _context.RefreshTokens
    //        .FirstOrDefaultAsync(rt => rt.Token == token);

    //    if (refreshToken != null)
    //    {
    //        refreshToken.Revoked = DateTime.UtcNow;
    //        await _context.SaveChangesAsync();
    //    }
    //}
}
