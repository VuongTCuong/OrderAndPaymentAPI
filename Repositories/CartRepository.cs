using Microsoft.EntityFrameworkCore;
using OrderAndPaymentAPI.Data;
using OrderAndPaymentAPI.Entities;

namespace OrderAndPaymentAPI.Repositories;

public interface ICartRepository
{
    Task<Cart?> GetByUserIdAsync(int userId);
    Task<Cart> AddAsync(Cart cart);
    Task<Cart> UpdateAsync(Cart cart);
    Task<bool> DeleteItemAsync(int userId, int productId);
    Task<bool> ClearAsync(int userId);
}

public class CartRepository: ICartRepository
{
    private readonly AppDbContext _context;

    public CartRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Cart?> GetByUserIdAsync(int userId)
    {
        return await _context.Cart
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .SingleOrDefaultAsync(c => c.UserId == userId);
    }

    public async Task<Cart> AddAsync(Cart cart)
    {
        _context.Cart.Add(cart);
        await _context.SaveChangesAsync();
        return cart;
    }

    public async Task<Cart> UpdateAsync(Cart cart)
    {
        // Cart được lấy từ GetByUserIdAsync và đang được EF theo dõi.
        cart.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return cart;
    }

    public async Task<bool> DeleteItemAsync(int userId, int productId)
    {
        var cart = await GetByUserIdAsync(userId);
        var item = cart?.CartItems.SingleOrDefault(ci => ci.ProductId == productId);
        if (cart == null || item == null)
        {
            return false;
        }

        _context.CartItem.Remove(item);
        cart.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ClearAsync(int userId)
    {
        var cart = await GetByUserIdAsync(userId);
        if (cart == null)
        {
            return false;
        }

        _context.CartItem.RemoveRange(cart.CartItems);
        cart.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }
}
