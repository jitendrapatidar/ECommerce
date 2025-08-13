using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Threading.Tasks;
using ECommerce.API.Controllers;
using ECommerce.Application.DTOs;
using ECommerce.Business.Interfaces;

namespace ECommerce.API.Tests;

public class OrdersControllerTests
{
    private readonly Mock<IOrderService> _mockOrderService;
    private readonly Mock<ILogger<OrdersController>> _mockLogger;
    private readonly OrdersController _controller;

    public OrdersControllerTests()
    {
        _mockOrderService = new Mock<IOrderService>();
        _mockLogger = new Mock<ILogger<OrdersController>>();
        _controller = new OrdersController(_mockOrderService.Object, _mockLogger.Object);

        // Mock User Claims for controller context
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "testuser")
        }, "mock"));

        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext() { User = user }
        };
    }
    [Fact]
    public async Task CreateOrder_ShouldReturnCreated_WhenOrderIsSuccessful()
    {
        // Arrange
        var orderDto = new OrderDto(0, 0, System.DateTime.UtcNow, 100m, "Pending", null);
        var createdOrder = new OrderDto(1, 1, System.DateTime.UtcNow, 100m, "Pending", null);

        _mockOrderService
            .Setup(s => s.CreateOrderAsync(1, orderDto))
            .ReturnsAsync(createdOrder);

        // Act
        var result = await _controller.CreateOrder(orderDto);

        // Assert
        var createdAtResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(_controller.GetOrder), createdAtResult.ActionName);
        Assert.Equal(createdOrder, createdAtResult.Value);
    }
    [Fact]
    public async Task CreateOrder_ShouldReturnInternalServerError_WhenExceptionThrown()
    {
        // Arrange
        var orderDto = new OrderDto(0, 0, System.DateTime.UtcNow, 100m, "Pending", null);

        _mockOrderService
            .Setup(s => s.CreateOrderAsync(1, orderDto))
            .ThrowsAsync(new System.Exception("Database error"));

        // Act
        var result = await _controller.CreateOrder(orderDto);

        // Assert
        var objResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, objResult.StatusCode);
        Assert.Equal("An error occurred while creating the order.", objResult.Value);
    }

    [Fact]
    public async Task GetOrder_ShouldReturnOk_WhenOrderExists()
    {
        // Arrange
        var orderId = 1;
        var order = new OrderDto(orderId, 1, System.DateTime.UtcNow, 50m, "Pending", null);

        _mockOrderService.Setup(s => s.GetOrderByIdAsync(orderId)).ReturnsAsync(order);

        // Act
        var result = await _controller.GetOrder(orderId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(order, okResult.Value);
    }

    [Fact]
    public async Task GetOrder_ShouldReturnNotFound_WhenOrderDoesNotExist()
    {
        // Arrange
        var orderId = 999;
        _mockOrderService.Setup(s => s.GetOrderByIdAsync(orderId)).ReturnsAsync((OrderDto?)null);

        // Act
        var result = await _controller.GetOrder(orderId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetUserOrders_ShouldReturnOk_WithOrders()
    {
        // Arrange
        var userId = 1;
        var orders = new[]
        {
                new OrderDto(1, userId, System.DateTime.UtcNow, 100m, "Pending", null),
                new OrderDto(2, userId, System.DateTime.UtcNow, 200m, "Shipped", null)
            };

        _mockOrderService.Setup(s => s.GetOrdersByUserAsync(userId)).ReturnsAsync(orders);

        // Act
        var result = await _controller.GetUserOrders(userId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(orders, okResult.Value);
    }

    [Fact]
    public async Task GetUserOrders_ShouldReturnInternalServerError_WhenExceptionThrown()
    {
        // Arrange
        var userId = 1;
        _mockOrderService.Setup(s => s.GetOrdersByUserAsync(userId))
            .ThrowsAsync(new System.Exception("DB error"));

        // Act
        var result = await _controller.GetUserOrders(userId);

        // Assert
        var objResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, objResult.StatusCode);
        Assert.Equal("An error occurred while retrieving the user's orders.", objResult.Value);
    }

}
