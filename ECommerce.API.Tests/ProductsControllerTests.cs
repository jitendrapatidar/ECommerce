using ECommerce.API.Controllers;
using ECommerce.Application.DTOs;
using ECommerce.Business.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ECommerce.API.Tests;

public class ProductsControllerTests
{
    private readonly Mock<IProductService> _mockProductService;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _mockProductService = new Mock<IProductService>();
        _controller = new ProductsController(_mockProductService.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk_WhenProductsExist()
    {
        // Arrange
        var products = new List<ProductDto>
                {
                    new ProductDto(
                        ProductId: 1,
                        Name: "Product1",
                        Description: "Desc1",
                        Price: 100m,
                        StockQuantity: 10,
                        CategoryId: 1,
                        CategoryName: "Category1",
                        CreatedAt: DateTime.UtcNow,
                        UpdatedAt: null
                    ),
                    new ProductDto(
                        ProductId: 2,
                        Name: "Product2",
                        Description: "Desc2",
                        Price: 200m,
                        StockQuantity: 5,
                        CategoryId: 2,
                        CategoryName: "Category2",
                        CreatedAt: DateTime.UtcNow,
                        UpdatedAt: null
                    )
                };

        


        _mockProductService.Setup(s => s.GetAllProductsAsync())
            .ReturnsAsync(products);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(products, okResult.Value);
    }
    [Fact]
    public async Task GetAll_ShouldReturnNotFound_WhenNoProductsExist()
    {
        // Arrange
        _mockProductService.Setup(s => s.GetAllProductsAsync())
            .ReturnsAsync(new List<ProductDto>());

        // Act
        var result = await _controller.GetAll();

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("No products found.", notFoundResult.Value);
    }
    [Fact]
    public async Task GetAll_ShouldReturnBadRequest_WhenArgumentExceptionThrown()
    {
        // Arrange
        _mockProductService.Setup(s => s.GetAllProductsAsync())
            .ThrowsAsync(new ArgumentException("Invalid request"));

        // Act
        var result = await _controller.GetAll();

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

        var json = JsonSerializer.Serialize(badRequestResult.Value);
        var doc = JsonDocument.Parse(json);
        var message = doc.RootElement.GetProperty("message").GetString();
 
        Assert.Equal("Invalid request", message);
    }

    [Fact]
    public async Task GetAll_ShouldReturnInternalServerError_WhenExceptionThrown()
    {
        // Arrange
        _mockProductService.Setup(s => s.GetAllProductsAsync())
            .ThrowsAsync(new Exception("DB down"));

        // Act
        var result = await _controller.GetAll();

        // Assert
        var objResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, objResult.StatusCode);
         
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_WhenProductExists()
    {
        // Arrange
        int productId = 1;
        
               var product = new ProductDto(
                ProductId: productId, // Generated
                Name: "Product1",
                Description: "Desc1",
                Price: 150m,
                StockQuantity: 20,
                CategoryId: 1,
                CategoryName: "Category1",
                CreatedAt: DateTime.UtcNow
            );
        _mockProductService.Setup(s => s.GetProductByIdAsync(productId))
            .ReturnsAsync(product);

        // Act
        var result = await _controller.GetById(productId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(product, okResult.Value);
    }

    [Fact]
    public async Task GetById_ShouldReturnBadRequest_WhenInvalidId()
    {
        // Act
        var result = await _controller.GetById(0);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        //  dynamic response = badRequest.Value;

        var json = JsonSerializer.Serialize(badRequest.Value);
        var doc = JsonDocument.Parse(json);
        var message = doc.RootElement.GetProperty("message").GetString();

        Assert.Equal("Invalid product ID.", message);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        int productId = 999;
        _mockProductService.Setup(s => s.GetProductByIdAsync(productId))
            .ReturnsAsync((ProductDto?)null);

        // Act
        var result = await _controller.GetById(productId);

        // Assert
        var notFound = Assert.IsType<NotFoundObjectResult>(result);
   
        var json = JsonSerializer.Serialize(notFound.Value);
        var doc = JsonDocument.Parse(json);
        var message = doc.RootElement.GetProperty("message").GetString();

        Assert.Equal($"Product with ID {productId} not found.", message);
    }

    [Fact]
    public async Task GetById_ShouldReturnInternalServerError_WhenExceptionThrown()
    {
        // Arrange
        int productId = 1;
        _mockProductService.Setup(s => s.GetProductByIdAsync(productId))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _controller.GetById(productId);

        // Assert
        var objResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, objResult.StatusCode);
        dynamic response = objResult.Value;
         
    }

    [Fact]
    public async Task Create_ShouldReturnCreated_WhenProductCreated()
    {
        // Arrange
        

        var newProductDto = new ProductDto(
                ProductId: 0, // Will be generated by service
                Name: "New Product",
                Description: "Desc",
                Price: 150m,
                StockQuantity: 20,
                CategoryId: 1,
                CategoryName: "Category1"
            );

        var createdProduct = new ProductDto(
            ProductId: 1, // Generated
            Name: "New Product",
            Description: "Desc",
            Price: 150m,
            StockQuantity: 20,
            CategoryId: 1,
            CategoryName: "Category1",
            CreatedAt: DateTime.UtcNow
        );

        _mockProductService.Setup(s => s.CreateProductAsync(newProductDto))
            .ReturnsAsync(createdProduct);

        // Act
        var result = await _controller.Create(newProductDto);

        // Assert
        var createdAtResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(_controller.GetById), createdAtResult.ActionName);
        Assert.Equal(createdProduct, createdAtResult.Value);
    }


}
