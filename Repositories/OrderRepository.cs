
using Microsoft.EntityFrameworkCore;

using OrderAndPaymentAPI.Data;
using OrderAndPaymentAPI.Entities;

namespace OrderAndPaymentAPI.Repositories;

public interface IOrderRepository
{
    
}
public class OrderRepository: IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }


    public async Task<Order?> GetOrderAsync(int userId)
    {
        return await _context.Order.Include(o => o.OrderItems).ThenInclude(oi => oi.Product).
                    SingleOrDefaultAsync(o => o.UserId == userId);
    }

}