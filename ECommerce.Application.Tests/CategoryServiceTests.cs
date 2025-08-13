using AutoMapper;
using ECommerce.Application.DTOs;
using ECommerce.Business.Service;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Tests;

public class CategoryServiceTests
{
    private readonly CategoryService _service;
    private readonly Mock<ICategoryRepository> _mockCategoryRepository = new();
    private readonly Mock<IProductRepository> _mockProductRepository = new();
    private readonly Mock<IMapper> _mockMapper = new();

    public CategoryServiceTests()
    {
        _service = new CategoryService(_mockCategoryRepository.Object, _mockProductRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task GetCategoryByIdAsync_ShouldReturnCategory_WhenExists()
    {
        var category = new Category { CategoryId = 1, Name = "Electronics" };
                        var categoryDto = new CategoryDto(
                    CategoryId: 1,
                    Name: "Electronics",
                    Description: "Electronic gadgets",
                    CreatedAt: DateTime.UtcNow,
                    UpdatedAt: null
                );

        _mockCategoryRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(category);
        _mockMapper.Setup(m => m.Map<CategoryDto>(category)).Returns(categoryDto);

        var result = await _service.GetCategoryByIdAsync(1);

        Assert.Equal(categoryDto.CategoryId, result.CategoryId);
        Assert.Equal(categoryDto.Name, result.Name);
    }

   

    [Fact]
    public async Task GetAllCategoriesAsync_ShouldReturnAllCategories()
    {
        var categories = new List<Category> { new Category { CategoryId = 1, Name = "Electronics" } };

      
                    var categoryDtos = new List<CategoryDto> { new CategoryDto(
                CategoryId: 1,
                Name: "Electronics",
                Description: "Electronics category",
                CreatedAt: DateTime.UtcNow,
                UpdatedAt: null
            )};

        _mockCategoryRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(categories);
        _mockMapper.Setup(m => m.Map<IEnumerable<CategoryDto>>(categories)).Returns(categoryDtos);

        var result = await _service.GetAllCategoriesAsync();

        Assert.Single(result);
        Assert.Equal("Electronics", result.First().Name);
    }

    [Fact]
    public async Task CreateCategoryAsync_ShouldReturnCreatedCategory()
    {
                    var dto = new CategoryDto(
                 CategoryId: 0,
                 Name: "Books",
                 Description: "Book category",
                 CreatedAt: DateTime.UtcNow,
                 UpdatedAt: null
             );
        var category = new Category { CategoryId = 1, Name = "Books" };

        _mockMapper.Setup(m => m.Map<Category>(dto)).Returns(category);
        _mockCategoryRepository.Setup(r => r.AddAsync(category ,It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _mockMapper.Setup(m => m.Map<CategoryDto>(category)).Returns(dto with { CategoryId = 1 });

        var result = await _service.CreateCategoryAsync(dto);

        Assert.Equal(1, result.CategoryId);
        Assert.Equal("Books", result.Name);
    }

    

}
