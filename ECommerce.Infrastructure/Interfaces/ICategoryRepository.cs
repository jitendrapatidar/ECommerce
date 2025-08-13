using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Interfaces;

public interface ICategoryRepository
{
    Task AddAsync(Category category, CancellationToken ct = default);

    Task DeleteAsync(Category category, CancellationToken ct = default);

    Task<IEnumerable<Category>> GetAllAsync(CancellationToken ct = default);

    Task<Category?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<Category>> GetByCategoryAsync(int categoryId, CancellationToken ct = default);

    Task UpdateAsync(Category category, CancellationToken ct = default);
}
