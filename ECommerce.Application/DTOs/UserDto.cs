namespace ECommerce.Application.DTOs;

public record UserDto(
    int UserId,
    string? Username,
    string? Email,
    string? PasswordHash,
    DateTime CreatedAt = default,
    DateTime? UpdatedAt = null
);
public record LoginDto(
    string? Email,
    string? PasswordHash
    );
public record registerDto(
    string? Username,
    string? Email,
    string? PasswordHash
    
);
