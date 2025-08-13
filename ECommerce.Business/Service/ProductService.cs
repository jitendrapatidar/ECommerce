using AutoMapper;
using ECommerce.Application.DTOs;
using ECommerce.Business.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Business.Service;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    public ProductService(IProductRepository productRepository,
                         ICategoryRepository categoryRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }
    public async Task<ProductDto> GetProductByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) throw new KeyNotFoundException("Product not found");

        return _mapper.Map<ProductDto>(product);
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<ProductDto> CreateProductAsync(ProductDto productDto)
    {
        var category = await _categoryRepository.GetByIdAsync(productDto.CategoryId.Value);
        if (category == null) throw new KeyNotFoundException("Category not found");

        var product = _mapper.Map<Product>(productDto);
        await _productRepository.AddAsync(product);

        var response = _mapper.Map<ProductDto>(product) with
        {
            CategoryName = category.Name
        };

        return response;
    }
    public async Task<ProductDto> UpdateProductAsync(int id, ProductDto productDto)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) throw new KeyNotFoundException("Product not found");

        if (productDto.CategoryId.HasValue)
        {
            var category = await _categoryRepository.GetByIdAsync(productDto.CategoryId.Value);
            if (category == null) throw new KeyNotFoundException("Category not found");
        }

        _mapper.Map(productDto, product);
        await _productRepository.UpdateAsync(product);

        var updatedProduct = await _productRepository.GetByIdAsync(id);
        return _mapper.Map<ProductDto>(updatedProduct);
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) throw new KeyNotFoundException("Product not found");

        await _productRepository.DeleteAsync(product);
        return true;
    }

    public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId)
    {
        var category = await _categoryRepository.GetByIdAsync(categoryId);
        if (category == null) throw new KeyNotFoundException("Category not found");

        var products = await _productRepository.GetByCategoryAsync(categoryId);

        var response = _mapper.Map<IEnumerable<ProductDto>>(products)
            .Select(product => product with { CategoryName = category.Name });

        return response;
    }
    


}
