using ECommerce.Application.DTOs;

namespace ECommerce.Business.Interfaces;

public interface IAuthService
{
    Task<UserDto> RegisterAsync(registerDto registerDto);
    Task<string> LoginAsync(LoginDto loginDto);

    Task<UserDto> GetCurrentUserAsync(int userId);

    Task ChangePasswordAsync(int userId, UserDto changePasswordDto);
}