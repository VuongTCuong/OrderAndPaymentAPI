using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OrderAndPaymentAPI.Services;
using OrderAndPaymentAPI.Entities;
using OrderAndPaymentAPI.Exceptions;
using Microsoft.AspNetCore.Authorization;

namespace OrderAndPaymentAPI.Web.Pages.CartPage;

[Authorize]
public class IndexModel : PageModel
{
    public Cart cart{  get; set; } = new();

    private readonly ICartService _cartService;

    public IndexModel(ICartService cartService)
    {
        _cartService = cartService;
    }
    public async Task OnGet()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        cart = string.IsNullOrEmpty(userIdClaim)
            ? new Cart()
            : await _cartService.GetCartByUserIdAsync(userIdClaim) ?? new Cart();
        
    }

    public async Task<IActionResult> OnPostUpdateQuantityAsync(int productId, int quantity)
    {
        if (User.Identity?.IsAuthenticated != true ||
            !int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
        {
            return RedirectToPage("/Account/Login");
        }

        if (ModelState.IsValid)
        {
            try
            {
                await _cartService.UpdateItemQuantityAsync(userId, productId, quantity);
                return RedirectToPage();
            }
            catch (AppException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
        }

        await OnGet();
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteCartItemAsync(int productId)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        await _cartService.DeleteItemAsync(userIdClaim, productId);
        await OnGet();
        return RedirectToPage();
    }
    public IActionResult OnPostCheckout()
    {
        // TODO: gọi POST /api/orders để tạo đơn hàng từ giỏ hàng hiện tại
        return RedirectToPage("/Orders/Index");
    }
}
