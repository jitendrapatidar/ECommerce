using ECommerce.API.Controllers;
using ECommerce.Application.DTOs;
using ECommerce.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
namespace ECommerce.API.Tests;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mockAuthService = new Mock<IAuthService>();
        _controller = new AuthController(_mockAuthService.Object);
    }
    
    [Fact]
    public async Task RegisterAsync_ShouldReturnOk_WhenUserRegistered()
    {
        // Arrange
        var registerDto = new registerDto("testuser", "test@example.com", "hashedpassword");
        var registeredUser = new UserDto (1,"testuser", "test@example.com", "hashedpassword");

        _mockAuthService.Setup(s => s.RegisterAsync(registerDto)).ReturnsAsync(registeredUser);

        // Act
        var result = await _controller.RegisterAsync(registerDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal(registeredUser, okResult.Value);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnBadRequest_WhenArgumentExceptionThrown()
    {
        

        // Arrange
        var registerDto = new registerDto("test", "test@example.com", "pass123");
       

        _mockAuthService
            .Setup(s => s.RegisterAsync(registerDto))
            .ThrowsAsync(new ArgumentException("User already exists"));

        // Act
        var result = await _controller.RegisterAsync(registerDto);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequest.StatusCode);
       
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnInternalServerError_WhenUnexpectedExceptionThrown()
    {
        // Arrange
        var registerDto = new registerDto("test", "test@example.com", "pass123");

        _mockAuthService
            .Setup(s => s.RegisterAsync(registerDto))
            .ThrowsAsync(new Exception("Database down"));

        // Act
        var result = await _controller.RegisterAsync(registerDto);

        // Assert
        var objResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, objResult.StatusCode);
         
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnOk_WithToken()
    {
        // Arrange
        var loginDto = new LoginDto("test@example.com", "pass123");
        var fakeToken = "jwt.token.here";

        _mockAuthService
            .Setup(s => s.LoginAsync(loginDto))
            .ReturnsAsync(fakeToken);

        // Act
        var result = await _controller.LoginAsync(loginDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        
    }

}
