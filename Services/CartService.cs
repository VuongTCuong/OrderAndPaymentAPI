using OrderAndPaymentAPI.Entities;
using OrderAndPaymentAPI.Repositories;
using OrderAndPaymentAPI.Exceptions;

namespace OrderAndPaymentAPI.Services;

public interface ICartService
{
    Task<Cart?> GetCartByUserIdAsync(string userIdClaim);
    Task<Cart> AddItemAsync(string userIdClaim, int productId, int quantity);
    Task<Cart> UpdateItemQuantityAsync(int userId, int productId, int quantity);
    Task DeleteItemAsync(string userIdClaim, int productId);
    Task ClearCartAsync(string userIdClaim);
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

    public int ChangeClaimtoInt(string userIdClaim)
    {
        
        if(userIdClaim == null)
        {
            throw new AppException("Người dùng không hợp lệ", 401);
        }
        if(!int.TryParse(userIdClaim, out int userId))
        {
            throw new AppException("Mã người dùng không hợp lệ", 400);
        }
        return userId;
    }
    public async Task<Cart?> GetCartByUserIdAsync(string userIdClaim)
    {
        int userId = ChangeClaimtoInt(userIdClaim);
        return await _cartRepo.GetByUserIdAsync(userId);
    }

    public async Task<Cart> AddItemAsync(string userIdClaim, int productId, int quantity)
    {
        int userId = ChangeClaimtoInt(userIdClaim);
        if (quantity <= 0)
        {
            throw new AppException("Số lượng phải lớn hơn 0.", 400);
        
        }

        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null)
        {
            throw new AppException("Không tìm thấy người dùng.", 404);
        }

        var product = await _productRepo.GetByIdAsync(productId);
        if (product == null)
        {
            throw new AppException("Không tìm thấy sản phẩm.", 404);
        }

        var cart = await _cartRepo.GetByUserIdAsync(userId);
        var item = cart?.CartItems.SingleOrDefault(ci => ci.ProductId == productId);
        var totalQuantity = (long)(item?.Quantity ?? 0) + quantity;
        if (totalQuantity > product.Stock)
        {
            throw new AppException("Số lượng vượt quá tồn kho.", 409);
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
            throw new AppException("Số lượng phải lớn hơn 0.", 400);
        }

        var cart = await _cartRepo.GetByUserIdAsync(userId);
        if (cart == null)
        {
            throw new AppException("Không tìm thấy giỏ hàng.", 404);
        }

        var item = cart.CartItems.SingleOrDefault(ci => ci.ProductId == productId);
        if (item == null)
        {
            throw new AppException("Sản phẩm không có trong giỏ hàng.", 404);
        }

        if (quantity > item.Product.Stock)
        {
            throw new AppException("Số lượng vượt quá tồn kho.", 409);
        }

        item.Quantity = quantity;
        return await _cartRepo.UpdateAsync(cart);
    }

    public async Task DeleteItemAsync(string userIdClaim, int productId)
    {
        int userId = ChangeClaimtoInt(userIdClaim);
        var cart = await _cartRepo.GetByUserIdAsync(userId);
        var item = cart?.CartItems.SingleOrDefault(ci => ci.ProductId == productId);
        if(cart==null)
        {
            throw new AppException("Không tìm thấy giỏ hàng người dùng", 404);
        }
        if(item==null)
        {
            throw new AppException("Không tìm thấy sản phẩm trong giỏ hàng", 404);
        }

        await _cartRepo.DeleteItemAsync(cart,item);
    }

    public async Task ClearCartAsync(string userIdClaim)
    {
        int userId = ChangeClaimtoInt(userIdClaim);

        var cart = await _cartRepo.GetByUserIdAsync(userId);
        if (cart == null)
        {
            throw new AppException("Không tìm thấy giỏ hàng người dùng", 404);
        }
        await _cartRepo.ClearAsync(cart);
    }
}
