using OrderAndPaymentAPI.Entities;
using OrderAndPaymentAPI.Repositories;

namespace OrderAndPaymentAPI.Services;

public interface ICartService
{
    Task<Cart?> GetCartByUserIdAsync(int userId);
    Task<Cart> AddItemAsync(int userId, int productId, int quantity);
    Task<Cart> UpdateItemQuantityAsync(int userId, int productId, int quantity);
    Task<bool> DeleteItemAsync(int userId, int productId);
    Task<bool> ClearCartAsync(int userId);
}

public class CartService: ICartService
{
    private readonly ICartRepository _cartRepo;
    private readonly IProductRepository _productRepo;
    private readonly IUserRepository _userRepo;

    public CartService(ICartRepository cartRepo, IProductRepository productRepo, IUserRepository userRepo)
    {
        _cartRepo = cartRepo;
        _productRepo = productRepo;
        _userRepo = userRepo;
    }

    public async Task<Cart?> GetCartByUserIdAsync(int userId)
    {
        return await _cartRepo.GetByUserIdAsync(userId);
    }

    public async Task<Cart> AddItemAsync(int userId, int productId, int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Số lượng phải lớn hơn 0.");
        }

        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException("Không tìm thấy người dùng.");
        }

        var product = await _productRepo.GetByIdAsync(productId);
        if (product == null)
        {
            throw new KeyNotFoundException("Không tìm thấy sản phẩm.");
        }

        var cart = await _cartRepo.GetByUserIdAsync(userId);
        var item = cart?.CartItems.SingleOrDefault(ci => ci.ProductId == productId);
        var totalQuantity = (long)(item?.Quantity ?? 0) + quantity;
        if (totalQuantity > product.Stock)
        {
            throw new InvalidOperationException("Số lượng vượt quá tồn kho.");
        }

        var isNewCart = cart == null;
        if (cart == null)
        {
            cart = new Cart
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        if (item == null)
        {
            cart.CartItems.Add(new CartItem
            {
                ProductId = productId,
                Product = product,
                Quantity = quantity
            });
        }
        else
        {
            item.Quantity = (int)totalQuantity;
        }

        if (isNewCart)
        {
            return await _cartRepo.AddAsync(cart);
        }

        return await _cartRepo.UpdateAsync(cart);
    }

    public async Task<Cart> UpdateItemQuantityAsync(int userId, int productId, int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Số lượng phải lớn hơn 0.");
        }

        var cart = await _cartRepo.GetByUserIdAsync(userId);
        if (cart == null)
        {
            throw new KeyNotFoundException("Không tìm thấy giỏ hàng.");
        }

        var item = cart.CartItems.SingleOrDefault(ci => ci.ProductId == productId);
        if (item == null)
        {
            throw new KeyNotFoundException("Sản phẩm không có trong giỏ hàng.");
        }

        if (quantity > item.Product.Stock)
        {
            throw new InvalidOperationException("Số lượng vượt quá tồn kho.");
        }

        item.Quantity = quantity;
        return await _cartRepo.UpdateAsync(cart);
    }

    public async Task<bool> DeleteItemAsync(int userId, int productId)
    {
        return await _cartRepo.DeleteItemAsync(userId, productId);
    }

    public async Task<bool> ClearCartAsync(int userId)
    {
        return await _cartRepo.ClearAsync(userId);
    }
}
