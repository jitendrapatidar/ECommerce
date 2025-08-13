using AutoMapper;
using ECommerce.Application.DTOs;
using ECommerce.Business.Service;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Interfaces;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ECommerce.Application.Tests;

public class ProductServiceTests
{
    private readonly ProductService _service;
    private readonly Mock<IProductRepository> _mockProductRepository = new();
    private readonly Mock<ICategoryRepository> _mockCategoryRepository = new();
    private readonly Mock<IMapper> _mockMapper = new();

    public ProductServiceTests()
    {
        _service = new ProductService(_mockProductRepository.Object, _mockCategoryRepository.Object, _mockMapper.Object);
    }
    [Fact]
    public async Task GetProductByIdAsync_ShouldReturnProduct_WhenExists()
    {
        // Arrange
        var product = new Product { ProductId = 1, Name = "TestProduct" };
        _mockProductRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(product);
        var productDto = new ProductDto(1, "TestProduct", null, 0, 0, null, null, default, null);
        _mockMapper.Setup(m => m.Map<ProductDto>(product)).Returns(productDto);

        // Act
        var result = await _service.GetProductByIdAsync(1);

        // Assert
        Assert.Equal(productDto.ProductId, result.ProductId);
    }
    [Fact]
    public async Task GetProductByIdAsync_ShouldThrow_WhenNotFound()
    {
        _mockProductRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((Product)null);
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetProductByIdAsync(1));
    }

    [Fact]
    public async Task GetAllProductsAsync_ShouldReturnAllProducts()
    {
        var products = new List<Product> { new Product { ProductId = 1, Name = "Test" } };
        _mockProductRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(products);
        var productDtos = new List<ProductDto> { new ProductDto(1, "Test", null, 0, 0, null, null, default, null) };
        _mockMapper.Setup(m => m.Map<IEnumerable<ProductDto>>(products)).Returns(productDtos);

        var result = await _service.GetAllProductsAsync();
        Assert.Single(result);
    }

    [Fact]
    public async Task CreateProductAsync_ShouldReturnCreatedProduct()
    {
        var dto = new ProductDto(0, "NewProduct", null, 100, 10, 1, null, default, null);
        var category = new Category { CategoryId = 1, Name = "Cat1" };
        var product = new Product { ProductId = 1, Name = "NewProduct" };
        var createdDto = dto with { ProductId = 1, CategoryName = category.Name };

        _mockCategoryRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(category);
        _mockMapper.Setup(m => m.Map<Product>(dto)).Returns(product);
        _mockProductRepository.Setup(r => r.AddAsync(product, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _mockMapper.Setup(m => m.Map<ProductDto>(product)).Returns(dto);

        var result = await _service.CreateProductAsync(dto);

        Assert.Equal(0, result.ProductId);
        Assert.Equal("Cat1", result.CategoryName);
    }

    [Fact]
    public async Task DeleteProductAsync_ShouldReturnTrue_WhenProductExists()
    {
        var product = new Product { ProductId = 1, Name = "Test" };
        _mockProductRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(product);
        _mockProductRepository.Setup(r => r.DeleteAsync(product, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _service.DeleteProductAsync(1);

        Assert.True(result);
    }
    [Fact]
    public async Task DeleteProductAsync_ShouldThrow_WhenProductNotFound()
    {
        _mockProductRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((Product)null);
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteProductAsync(1));
    }

    [Fact]
    public async Task GetProductsByCategoryAsync_ShouldReturnProductsWithCategoryName()
    {
        var category = new Category { CategoryId = 1, Name = "Cat1" };
        var products = new List<Product> { new Product { ProductId = 1, Name = "Test" } };
        var productDtos = new List<ProductDto> { new ProductDto(1, "Test", null, 0, 0, 1, null, default, null) };

        _mockCategoryRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(category);
        _mockProductRepository.Setup(r => r.GetByCategoryAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(products);
        _mockMapper.Setup(m => m.Map<IEnumerable<ProductDto>>(products)).Returns(productDtos);

        var result = await _service.GetProductsByCategoryAsync(1);

        Assert.All(result, p => Assert.Equal("Cat1", p.CategoryName));
    }
}
