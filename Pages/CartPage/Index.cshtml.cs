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
    private string? userIdClaim => User.FindFirstValue(ClaimTypes.NameIdentifier); 
    private readonly ICartService _cartService;
    private readonly IOrderService _orderService;
    public IndexModel(ICartService cartService, IOrderService orderService)
    {
        _cartService = cartService;
        _orderService = orderService;
    }
    public async Task OnGet()
    {
        cart = string.IsNullOrEmpty(userIdClaim)
            ? new Cart()
            : await _cartService.GetCartByUserIdAsync(userIdClaim) ?? new Cart();
        
    }

    public async Task<IActionResult> OnPostUpdateQuantityAsync(int productId, int quantity)
    {
        if (User.Identity?.IsAuthenticated != true ||
            !int.TryParse(userIdClaim, out var userId))
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
        await _cartService.DeleteItemAsync(userIdClaim, productId);
        await OnGet();
        return RedirectToPage();
    }
    
    public async Task<IActionResult> OnPostCheckout()
    {
        await _orderService.AddOrderAsync(userIdClaim);
        return RedirectToPage("/Orders/Index");
    }

   
}
