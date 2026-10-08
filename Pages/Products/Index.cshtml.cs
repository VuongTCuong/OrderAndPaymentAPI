using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OrderAndPaymentAPI.Entities;
using OrderAndPaymentAPI.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using OrderAndPaymentAPI.Exceptions;

namespace OrderAndPaymentAPI.Web.Pages.Products;

public class IndexModel : PageModel
{
    private readonly IProductService _productService;
    private readonly ICartService _cartService;
    public List<Product> Products { get; set; } = new();

    public IndexModel(IProductService productService, ICartService cartService)
    {
        _productService = productService;
        _cartService = cartService;
    }

    public async Task OnGetAsync()
    {
        Products = await _productService.GetAllProductsAsync();
    }

    public async Task<IActionResult> OnPostAsync(int productId, int quantity = 1)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (User.Identity?.IsAuthenticated != true || string.IsNullOrEmpty(userIdClaim))
        {
            return RedirectToPage("/Account/Login");
        }

        if (!ModelState.IsValid)
        {
            await OnGetAsync();
            return Page();
        }

        try
        {
            await _cartService.AddItemAsync(userIdClaim, productId, quantity);
        }
        catch (AppException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await OnGetAsync();
            return Page();
        }

        return RedirectToPage("/CartPage/Index");
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
