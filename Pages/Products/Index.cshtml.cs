using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OrderAndPaymentAPI.Web.Models;
using OrderAndPaymentAPI.Web.Services;

namespace OrderAndPaymentAPI.Web.Pages.Products;

public class IndexModel : PageModel
{
    public List<ProductVm> Products { get; set; } = new();

    public void OnGet()
    {
        // TODO: thay bằng gọi GET /api/products từ OrderAndPaymentAPI
        Products = SampleData.Products;
    }

    public IActionResult OnPost(int productId)
    {
        // TODO: gọi POST /api/cart/items để thêm sản phẩm vào giỏ hàng thật
        return RedirectToPage("/Cart/Index");
    }
}
