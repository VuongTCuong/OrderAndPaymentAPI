using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OrderAndPaymentAPI.Entities;
using OrderAndPaymentAPI.Exceptions;
using OrderAndPaymentAPI.Services;


namespace OrderAndPaymentAPI.Web.Pages.Orders;

public class IndexModel : PageModel
{
    public bool IsAdmin => User.IsInRole("admin");
    private readonly IOrderService _orderService;
    public List<Order> Orders { get; set; } = new();
    private string? userIdClaim => User.FindFirstValue(ClaimTypes.NameIdentifier); 
    public IndexModel(IOrderService orderService)
    {
        _orderService = orderService;
    }
    public async Task OnGet()
    {
        Orders = IsAdmin
            ? await _orderService.GetAllOrdersAsync()
            : await _orderService.GetOrderByUserIdAsync(userIdClaim);
    }


    public async Task<IActionResult> OnPostUpdateOrderStatus(int orderId, string status)
    {
        if (!IsAdmin)
            return Forbid();

        await _orderService.UpdateOrderStatusAsync(orderId, status);
        return RedirectToPage("/Orders/Index");
    }

    public async Task<IActionResult> OnPostCancelOrderAsync(int orderId, string? cancelReason, string? otherReason)
    {
        var claim = userIdClaim;
        if (User.Identity?.IsAuthenticated != true || string.IsNullOrWhiteSpace(claim))
        {
            return Challenge();
        }

        var reason = cancelReason switch
        {
            "change-items" => "Tôi muốn thay đổi sản phẩm hoặc số lượng",
            "change-address" => "Tôi muốn thay đổi địa chỉ giao hàng",
            "no-longer-needed" => "Tôi không còn nhu cầu mua hàng",
            "other" => otherReason?.Trim(),
            _ => null
        };

        if (orderId <= 0)
            ModelState.AddModelError(string.Empty, "Mã đơn hàng không hợp lệ.");
        if (string.IsNullOrWhiteSpace(reason))
            ModelState.AddModelError(string.Empty, "Vui lòng chọn hoặc nhập lý do huỷ đơn hàng.");
        else if (reason.Length > 500)
            ModelState.AddModelError(string.Empty, "Lý do huỷ không được vượt quá 500 ký tự.");

        Orders = await _orderService.GetOrderByUserIdAsync(claim);
        var order = Orders.FirstOrDefault(o => o.Id == orderId);
        if (orderId > 0 && order == null)
            ModelState.AddModelError(string.Empty, "Không tìm thấy đơn hàng của bạn.");
        else if (order != null && order.Status is not ("Pending" or "Đang xử lý"))
            ModelState.AddModelError(string.Empty, "Chỉ có thể huỷ đơn hàng đang xử lý.");

        if (!ModelState.IsValid)
            return Page();

        try
        {
            await _orderService.CancelOrderAsync(claim, orderId, reason!);
        }
        catch (AppException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }

        return RedirectToPage("/Orders/Index");
    }
    public static string BadgeClass(string status) => status switch
    {
        "Completed" => "badge-completed",
        "Pending" => "badge-pending",
        "Confirmed" or "Đã xác nhận" => "badge-confirmed",
        "Cancelled" or "Đã huỷ" or "Đã hủy" => "badge-cancelled",
        _ => "badge-pending"
    };

    public static string StatusLabel(string status) => status switch
    {
        "Completed" => "Hoàn thành",
        "Pending" => "Đang xử lý",
        "Confirmed" or "Đã xác nhận" => "Đã xác nhận",
        "Cancelled" or "Đã huỷ" or "Đã hủy" => "Đã hủy",
        _ => status
    };
}
