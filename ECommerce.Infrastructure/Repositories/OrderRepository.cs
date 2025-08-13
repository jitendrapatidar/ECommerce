using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Data;
using ECommerce.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly ECommerceDbContext _dbContext;
    public OrderRepository(ECommerceDbContext dbContext) => _dbContext = dbContext;

    public async Task<Order> AddAsync(Order order)
    {
        await _dbContext.Orders.AddAsync(order);
        await _dbContext.SaveChangesAsync();
        return order;
    }

    public Task<Order?> GetByIdAsync(int id) =>
        _dbContext.Orders.Include(o => o.OrderItems).AsNoTracking().FirstOrDefaultAsync(o => o.OrderId == id);

    public Task<IEnumerable<Order>> GetByUserIdAsync(int userId) =>
        _dbContext.Orders.Include(o => o.OrderItems).Where(o => o.UserId == userId).AsNoTracking().ToListAsync().ContinueWith(t => (IEnumerable<Order>)t.Result);

    public async Task AddAsync(Order order, CancellationToken ct = default)
    {
        await _dbContext.Orders.AddAsync(order, ct);
        await _dbContext.SaveChangesAsync(ct);
    }
    public async Task DeleteAsync(Order order, CancellationToken ct = default)
    {
        _dbContext.Orders.Remove(order);
        await _dbContext.SaveChangesAsync(ct);
    }
    public Task<IEnumerable<Order>> GetAllAsync(CancellationToken ct = default) =>
       _dbContext.Orders.AsNoTracking().ToListAsync(ct).ContinueWith(t => (IEnumerable<Order>)t.Result);

}
