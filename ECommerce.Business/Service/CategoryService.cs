using AutoMapper;
using ECommerce.Application.DTOs;
using ECommerce.Business.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Interfaces;

namespace ECommerce.Business.Service;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public CategoryService(ICategoryRepository categoryRepository,
                         IProductRepository productRepository,
                         IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<CategoryDto> GetCategoryByIdAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null) throw new KeyNotFoundException("Category not found");

        return _mapper.Map<CategoryDto>(category);
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<CategoryDto>>(categories);
    }

    public async Task<CategoryDto> CreateCategoryAsync(CategoryDto categoryDto)
    {
        var category = _mapper.Map<Category>(categoryDto);
        await _categoryRepository.AddAsync(category);

        return _mapper.Map<CategoryDto>(category);
    }

    public async Task UpdateCategoryAsync(CategoryDto categoryDto)
    {
        var existingCategory = await _categoryRepository.GetByIdAsync(categoryDto.CategoryId);
        if (existingCategory == null) throw new KeyNotFoundException("Category not found");

        _mapper.Map(categoryDto, existingCategory);
        await _categoryRepository.UpdateAsync(existingCategory);
    }

    public async Task DeleteCategoryAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null) throw new KeyNotFoundException("Category not found");

        // Check if category has products
        var products = await _productRepository.GetByCategoryAsync(id);  //GetProductsByCategoryAsync
        if (products.Any())
            throw new InvalidOperationException("Cannot delete category with existing products");

        await _categoryRepository.DeleteAsync(category);
    }
    public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId)
    {
        var products = await _productRepository.GetByCategoryAsync(categoryId);
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

}