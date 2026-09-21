using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OrderAndPaymentAPI.Web.Models;
using OrderAndPaymentAPI.Web.Services;

namespace OrderAndPaymentAPI.Web.Pages.Cart;

public class IndexModel : PageModel
{
    public CartVm Cart { get; set; } = new();

    public void OnGet()
    {
        // TODO: thay bằng gọi GET /api/cart từ OrderAndPaymentAPI
        Cart = SampleData.Cart;
    }

    public IActionResult OnPostCheckout()
    {
        // TODO: gọi POST /api/orders để tạo đơn hàng từ giỏ hàng hiện tại
        return RedirectToPage("/Orders/Index");
    }
}
