namespace ECommerce.Application.DTOs;

public record UserInRoleDto(
    int UserId,
    int RoleId,
    DateTime AssignedAt = default
);  
