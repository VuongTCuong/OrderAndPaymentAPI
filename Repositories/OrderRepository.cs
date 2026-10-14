
using Microsoft.EntityFrameworkCore;

using OrderAndPaymentAPI.Data;
using OrderAndPaymentAPI.Entities;

namespace OrderAndPaymentAPI.Repositories;

public interface IOrderRepository
{
    public Task<List<Order>> GetAllOrdersAsync();
    public Task<Order?> GetOrderAsync(int userId);
    public Task<List<Order>> GetOrderByUserIdAsync(int userId);
    public Task<Order?> AddOrderAsync(Order order);

    public Task<Order?> UpdateOrderAsync(Order order);
}
public class OrderRepository: IOrderRepository
{
    public Task<List<Order>> GetAllOrdersAsync() => _context.Order
        .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
        .AsNoTracking().OrderByDescending(o => o.CreatedAt).ToListAsync();
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }


    public async Task<List<Order>> GetOrderByUserIdAsync(int userId)
    {
        return await _context.Order.Include(o => o.OrderItems).ThenInclude(oi => oi.Product).
                     Where(o => o.UserId == userId).AsNoTracking().ToListAsync();
                
    }

    public async Task<Order?> GetOrderAsync(int orderId)
    {
        return await _context.Order.FirstOrDefaultAsync(o => o.Id == orderId);
                
    }
    public async Task<Order?> AddOrderAsync(Order order)
    {
        _context.Order.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<Order?> UpdateOrderAsync(Order order)
    {
        await _context.SaveChangesAsync();
        return order;
    }
}
