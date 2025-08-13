namespace ECommerce.Application.DTOs;
public record ProductDto(
    int ProductId,
    string? Name,
    string? Description,
    decimal Price,
    int StockQuantity,
     int? CategoryId,
     string? CategoryName,
     DateTime CreatedAt = default,
    DateTime? UpdatedAt = null
);
