using Microsoft.AspNetCore.Mvc.RazorPages;
using OrderAndPaymentAPI.Web.Models;
using OrderAndPaymentAPI.Web.Services;

namespace OrderAndPaymentAPI.Web.Pages.Orders;

public class IndexModel : PageModel
{
    public List<OrderVm> Orders { get; set; } = new();

    public void OnGet()
    {
        // TODO: thay bằng gọi GET /api/orders từ OrderAndPaymentAPI
        Orders = SampleData.Orders;
    }

    public static string BadgeClass(string status) => status switch
    {
        "Completed" => "badge-completed",
        "Pending" => "badge-pending",
        "Cancelled" => "badge-cancelled",
        _ => "badge-pending"
    };

    public static string StatusLabel(string status) => status switch
    {
        "Completed" => "Hoàn thành",
        "Pending" => "Đang xử lý",
        "Cancelled" => "Đã hủy",
        _ => status
    };
}
