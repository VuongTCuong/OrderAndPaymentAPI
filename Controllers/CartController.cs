using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderAndPaymentAPI.DTO;
using OrderAndPaymentAPI.Services;

namespace OrderAndPaymentAPI.Controllers;

[ApiController]
[Route("api/cart")]
public class CartController: ControllerBase
{

    private readonly ICartService _cartService;    
    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet]
    [Authorize(Roles = "user")]
    public async Task<IActionResult> GetCart()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var cart = await _cartService.GetCartByUserIdAsync(userIdClaim);
        return Ok(cart?.CartItems.Select(item => new
            {
                item.CartId,
                item.ProductId,
                item.Quantity,
                ProductName = item.Product.Name,
                Price = item.Product.Price
            }).ToList());
    }

    
    [Authorize(Roles = "user")]
    [HttpPost("add")]
    public async Task<IActionResult> AddToCart(AddtoCartReqeustDTO atcRquestDTO)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var cart = await _cartService.AddItemAsync(userIdClaim, atcRquestDTO.productId, atcRquestDTO.quantity);
        return Ok(cart?.CartItems.Select(item => new
            {
                item.CartId,
                item.ProductId,
                item.Quantity,
                ProductName = item.Product.Name,
                Price = item.Product.Price
            }).ToList());
    }
}