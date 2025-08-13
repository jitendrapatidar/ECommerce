using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Data;
using ECommerce.Infrastructure.Interfaces;
using System;
 using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ECommerceDbContext _dbContext;
    public ProductRepository(ECommerceDbContext dbContext) => _dbContext = dbContext;

    public async Task AddAsync(Product product, CancellationToken ct = default)
    {
        await _dbContext.Products.AddAsync(product, ct);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Product product, CancellationToken ct = default)
    {
        _dbContext.Products.Remove(product);
        await _dbContext.SaveChangesAsync(ct);
    }

    public Task<IEnumerable<Product>> GetAllAsync(CancellationToken ct = default) =>
        _dbContext.Products.Include(p => p.Category).AsNoTracking().ToListAsync(ct).ContinueWith(t => (IEnumerable<Product>)t.Result);

    public Task<Product?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _dbContext.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.ProductId == id, ct);

    public Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId, CancellationToken ct = default) =>
        _dbContext.Products.Where(p => p.CategoryId == categoryId).Include(p => p.Category).AsNoTracking().ToListAsync(ct).ContinueWith(t => (IEnumerable<Product>)t.Result);

    public async Task UpdateAsync(Product product, CancellationToken ct = default)
    {
        _dbContext.Products.Update(product);
        await _dbContext.SaveChangesAsync(ct);
    }
}
