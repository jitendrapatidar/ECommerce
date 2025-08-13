using ECommerce.Application.DTOs;

namespace ECommerce.Business.Interfaces;

public interface IOrderService
{
    Task<OrderDto> GetOrderByIdAsync(int id);
    Task<IEnumerable<OrderDto>> GetOrdersByUserAsync(int userId);
    Task<OrderDto> CreateOrderAsync(int userId, OrderDto orderDto);
    Task CancelOrderAsync(int orderId);
    Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
}