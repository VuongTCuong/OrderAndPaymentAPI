
using OrderAndPaymentAPI.Entities;
using OrderAndPaymentAPI.Exceptions;
using OrderAndPaymentAPI.Repositories;

namespace OrderAndPaymentAPI.Services;

public interface IOrderService
{
    public Task<List<Order>> GetAllOrdersAsync();
    public Task<List<Order>> GetOrderByUserIdAsync(string userIdClaim);
    public Task<Order?> AddOrderAsync(string userIdClaim);
    public Task<Order?> UpdateOrderStatusAsync(int orderId, string status);
    public Task<Order?> CancelOrderAsync(string userIdClaim, int orderId, string CancellationReason);
}
public class OrderService: IOrderService
{
    public Task<List<Order>> GetAllOrdersAsync() => _orderRepo.GetAllOrdersAsync();
    private readonly IOrderRepository _orderRepo;
    private readonly IUserRepository _userRepo;
    private readonly ICartRepository _cartRepo;
    
    public OrderService(IOrderRepository orderRepo, IUserRepository userRepo, ICartRepository cartRepo)
    {
        _orderRepo = orderRepo;
        _userRepo = userRepo;
        _cartRepo = cartRepo;
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
    
    public async Task<List<Order>> GetOrderByUserIdAsync(string userIdClaim)
    {       
        int userId = ChangeClaimtoInt(userIdClaim);
        return await _orderRepo.GetOrderByUserIdAsync(userId);
    }

    public async Task<Order?> AddOrderAsync(string userIdClaim)
    {
        int userId = ChangeClaimtoInt(userIdClaim);

        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null)
        {
            throw new AppException("Không tìm thấy người dùng.", 404);
        }

        var cart = await _cartRepo.GetByUserIdAsync(userId);
        if (cart == null)
        {
            throw new AppException("Không tìm thấy giỏ hàng.", 404);
        }

        var order = new Order
        {
            UserId = userId,
            Status = "Đang xử lý",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        order.OrderItems = cart.CartItems.Select(oi => new OrderItem
        {
            OrderId = order.Id,
            ProductId = oi.ProductId,
            Quantity = oi.Quantity,
            Price = oi.Product.Price
        }).ToList();

        order.TotalAmount = order.OrderItems.Sum(oi => oi.Quantity * oi.Price);
        
        //clear cart
        await _cartRepo.ClearAsync(cart);

        //add to order
        await _orderRepo.AddOrderAsync(order);
        return null;
    }

    public async Task<Order?> CancelOrderAsync(string userIdClaim, int orderId, string CancellationReason)
    {
        int userId = ChangeClaimtoInt(userIdClaim);
        var user = await _userRepo.GetByIdAsync(userId);

        if(user == null)
        {
            throw new AppException("Người dùng không tồn tại", 404);
        }

        var order = await _orderRepo.GetOrderAsync(orderId);
        if(order == null)
        {
            throw new AppException("Không tìm thấy đơn hàng để huỷ",404);
        }
        if (order.UserId != user.Id && user.Role == "user")
        {
            throw new AppException("Bạn không có quyền huỷ đơn hàng này", 403);
        }

        if(order.Status != "Đang xử lý")
        {
            throw new AppException("Đơn hàng không thể huỷ", 409);
        }
        
        order.UpdatedAt = DateTime.UtcNow;
        order.CanceledAt = DateTime.UtcNow;
        order.CanceledReason = CancellationReason;
        order.Status = "Đã huỷ";
        return await _orderRepo.UpdateOrderAsync(order);
    }

    public async Task<Order?> UpdateOrderStatusAsync(int orderId, string status)
    {
        var order = await _orderRepo.GetOrderAsync(orderId);
        if(order == null)
        {
            throw new AppException("Không tìm thấy đơn hàng để cập nhật trạng thái",404);
        }
        
        order.Status = status;
        return await _orderRepo.UpdateOrderAsync(order);
    }
}
