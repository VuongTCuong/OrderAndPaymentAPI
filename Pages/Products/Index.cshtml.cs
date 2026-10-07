using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OrderAndPaymentAPI.Entities;
using OrderAndPaymentAPI.Services;
using Microsoft.AspNetCore.Authorization;

namespace OrderAndPaymentAPI.Web.Pages.Products;

public class IndexModel : PageModel
{
    private readonly IProductService _productService;

    public List<Product> Products { get; set; } = new();

    public IndexModel(IProductService productService)
    {
        _productService = productService;
    }

    public async Task OnGetAsync()
    {
        Products = await _productService.GetAllProductsAsync();
    }

    public IActionResult OnPost(int productId)
    {
        if (!(User.Identity?.IsAuthenticated ?? false))
        {
            return RedirectToPage("/Account/Login");
        }

        // TODO: gọi POST /api/cart/items để thêm sản phẩm vào giỏ hàng thật
        return RedirectToPage("/Cart/Index");
    }
    
    public async Task<IActionResult> OnPostDeleteAsync(int Id)
    {
        if (!User.IsInRole("admin"))
    {
        return Forbid(); 
    }

    await _productService.DeleteProductById(Id);
    return RedirectToPage();
    }
}
