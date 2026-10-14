using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OrderAndPaymentAPI.Entities;
using OrderAndPaymentAPI.Services;

using System.Security.Claims;
using OrderAndPaymentAPI.Exceptions;

namespace OrderAndPaymentAPI.Web.Pages.Products;

public class IndexModel : PageModel
{
    private readonly IProductService _productService;
    private readonly ICartService _cartService;
    public List<Product> Products { get; set; } = new();
    
    private int pageSize = 6;
    public int totalPage => (Products.Count/pageSize)+1;

    public IndexModel(IProductService productService, ICartService cartService)
    {
        _productService = productService;
        _cartService = cartService;
    }

    public async Task OnGetAsync(int pageNumber)
    {   
        pageNumber = Math.Max(1, pageNumber);
        Products = await _productService.GetAllProductsAsync();
        Products = Products.Skip((pageNumber-1)*pageSize).Take(pageSize).ToList();
    }

    public async Task<IActionResult> OnPostAsync(int pageNumber, int productId, int quantity = 1)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (User.Identity?.IsAuthenticated != true || string.IsNullOrEmpty(userIdClaim))
        {
            return RedirectToPage("/Account/Login");
        }

        if (!ModelState.IsValid)
        {
            await OnGetAsync(pageNumber);
            return Page();
        }

        try
        {
            await _cartService.AddItemAsync(userIdClaim, productId, quantity);
        }
        catch (AppException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await OnGetAsync(pageNumber);
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
