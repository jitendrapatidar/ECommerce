using AutoMapper;
using ECommerce.Application.DTOs;
using ECommerce.Business.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Business.Service;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUserRepository _userRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    public OrderService(IOrderRepository orderRepository, IUserRepository userRepository, IProductRepository productRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _userRepository = userRepository;
        _productRepository = productRepository;
        _mapper = mapper;
    }
    public async Task<OrderDto> GetOrderByIdAsync(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null) throw new KeyNotFoundException("Order not found");

        return _mapper.Map<OrderDto>(order);
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByUserAsync(int userId)
    {
        var orders = await _orderRepository.GetByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<OrderDto>>(orders);
    }

    public async Task<OrderDto> CreateOrderAsync(int userId,OrderDto orderDto)
    {
        // Validate user exists
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) throw new KeyNotFoundException("User not found");

        var order = new Order
        {
            UserId = orderDto.UserId,
            OrderDate = DateTime.UtcNow,
            OrderItems = new List<OrderItem>()
        };

        decimal totalAmount = 0;

        foreach (var item in orderDto.OrderItems)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId);
            if (product == null)
                throw new KeyNotFoundException($"Product with ID {item.ProductId} not found");

            if (product.StockQuantity < item.Quantity)
                throw new InvalidOperationException($"Insufficient stock for product {product.Name}");

            var orderItem = new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = product.Price
            };

            order.OrderItems.Add(orderItem);
            totalAmount += product.Price * item.Quantity;

            // Update product stock
            product.StockQuantity -= item.Quantity;
            await _productRepository.UpdateAsync(product);
        }

        order.TotalAmount = totalAmount;
        await _orderRepository.AddAsync(order);

        return _mapper.Map<OrderDto>(order);
    }

    public async Task CancelOrderAsync(int orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null) throw new KeyNotFoundException("Order not found");

        // Restore product quantities
        foreach (var item in order.OrderItems)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId);
            if (product != null)
            {
                product.StockQuantity += item.Quantity;
                await _productRepository.UpdateAsync(product);
            }
        }

        await _orderRepository.DeleteAsync(order);
    }

    public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
    {
        var orders = await _orderRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<OrderDto>>(orders);
    }

}
    