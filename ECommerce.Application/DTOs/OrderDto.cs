namespace ECommerce.Application.DTOs;

public record OrderDto(
    int OrderId,
    int UserId,
    DateTime OrderDate = default,
    decimal TotalAmount = 0.0m,
    string Status = "Pending",
    List<OrderItemDto> OrderItems = null! // or new()
);