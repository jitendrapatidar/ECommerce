namespace ECommerce.Application.DTOs;


public record OrderItemDto(
    int OrderItemId,
    int OrderId,
    int ProductId,
    int Quantity,
    decimal UnitPrice
);
 