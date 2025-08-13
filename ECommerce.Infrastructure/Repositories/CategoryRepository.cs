using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Data;
using ECommerce.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace ECommerce.Infrastructure.Repositories;

public class CategoryRepository: ICategoryRepository
{
    private readonly ECommerceDbContext _dbContext;
    public CategoryRepository(ECommerceDbContext dbContext) => _dbContext = dbContext;

    public async Task AddAsync(Category category, CancellationToken ct = default)
    {
        await _dbContext.Categories.AddAsync(category, ct);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Category category, CancellationToken ct = default)
    {
        _dbContext.Categories.Remove(category);
        await _dbContext.SaveChangesAsync(ct);
    }
   
    public Task<IEnumerable<Category>> GetAllAsync(CancellationToken ct = default) =>
         _dbContext.Categories.AsNoTracking()
        .ToListAsync(ct).ContinueWith(t => (IEnumerable<Category>)t.Result);
     
    public Task<Category?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _dbContext.Categories.FirstOrDefaultAsync(p => p.CategoryId == id, ct);

    public Task<IEnumerable<Category>> GetByCategoryAsync(int categoryId, CancellationToken ct = default) =>
        _dbContext.Categories.Where(p => p.CategoryId == categoryId)
          .AsNoTracking().ToListAsync(ct).ContinueWith(t => (IEnumerable<Category>)t.Result);

    public async Task UpdateAsync(Category category, CancellationToken ct = default)
    {
        _dbContext.Categories.Update(category);
        await _dbContext.SaveChangesAsync(ct);
    }

}
