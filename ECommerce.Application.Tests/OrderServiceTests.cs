using AutoMapper;
using ECommerce.Application.DTOs;
using ECommerce.Business.Service;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ECommerce.Application.Tests;

public class OrderServiceTests
{
    private readonly OrderService _service;
    private readonly Mock<IOrderRepository> _mockOrderRepo = new();
    private readonly Mock<IUserRepository> _mockUserRepo = new();
    private readonly Mock<IProductRepository> _mockProductRepo = new();
    private readonly Mock<IMapper> _mockMapper = new();

    public OrderServiceTests()
    {
        _service = new OrderService(_mockOrderRepo.Object, _mockUserRepo.Object, _mockProductRepo.Object, _mockMapper.Object);
    }
    [Fact]
    public async Task GetOrderByIdAsync_ShouldReturnOrder_WhenExists()
    {
        var order = new Order { OrderId = 1, UserId = 1 };
        var orderDto = new OrderDto(1, 1, DateTime.UtcNow, 0, null);

        _mockOrderRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(order);
        _mockMapper.Setup(m => m.Map<OrderDto>(order)).Returns(orderDto);

        var result = await _service.GetOrderByIdAsync(1);

        Assert.Equal(orderDto.OrderId, result.OrderId);
        Assert.Equal(orderDto.UserId, result.UserId);
    }
    [Fact]
    public async Task GetOrdersByUserAsync_ShouldReturnOrders()
    {
        var orders = new List<Order> { new Order { OrderId = 1, UserId = 1 } };
        var orderDtos = new List<OrderDto> { new OrderDto(1, 1, DateTime.UtcNow, 0, null) };

        _mockOrderRepo.Setup(r => r.GetByUserIdAsync(1)).ReturnsAsync(orders);
        _mockMapper.Setup(m => m.Map<IEnumerable<OrderDto>>(orders)).Returns(orderDtos);

        var result = await _service.GetOrdersByUserAsync(1);

        Assert.Single(result);
        Assert.Equal(1, result.First().OrderId);
    }
    [Fact]
    public async Task CreateOrderAsync_ShouldCreateOrder_WhenValid()
    {
        var user = new User
        {
            UserId = 1,
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hashedpassword",
            CreatedAt = DateTime.UtcNow
          
        };
        var product = new Product { ProductId = 1, Name = "TestProduct", Price = 10, StockQuantity = 5 };
       
        var orderDto = new OrderDto(0, 1, DateTime.UtcNow, 0, "pending",
    new List<OrderItemDto> { new OrderItemDto(1, 1, 1, 1, 100m) });

       
        var order = new Order { OrderId = 1, UserId = 1, TotalAmount = 10, OrderItems = new List<OrderItem> { new OrderItem { ProductId = 1, Quantity = 1, UnitPrice = 10 } } };

        _mockUserRepo.Setup(r => r.GetByIdAsync(1,It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _mockProductRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(product);
        _mockProductRepo.Setup(r => r.UpdateAsync(product, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _mockOrderRepo.Setup(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _mockMapper.Setup(m => m.Map<OrderDto>(It.IsAny<Order>())).Returns(orderDto with { OrderId = 1 });
       
        var result = await _service.CreateOrderAsync(1, orderDto);

        Assert.Equal(1, result.OrderId);
        Assert.Single(result.OrderItems);
    }

    [Fact]
    public async Task CreateOrderAsync_ShouldThrow_WhenUserNotFound()
    {
        _mockUserRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((User)null);
        var dto = new OrderDto(0, 1, DateTime.UtcNow, 0, null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.CreateOrderAsync(1, dto));
    }

    [Fact]
    public async Task GetOrderByIdAsync_ShouldThrow_WhenNotFound()
    {
        _mockOrderRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Order)null);
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetOrderByIdAsync(1));
    }

}
