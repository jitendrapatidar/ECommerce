namespace ECommerce.Application.DTOs;
public record  CategoryDto(
    int CategoryId, 
    string? Name, 
    string? Description, 
    DateTime CreatedAt, 
    DateTime? UpdatedAt);
 