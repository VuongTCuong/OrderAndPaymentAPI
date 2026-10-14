

using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderAndPaymentAPI.DTO;
using OrderAndPaymentAPI.Services;

namespace OrderAndPaymentAPI.Controllers;

[ApiController]
[Route("api/order")]
public class OrderController: ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [Authorize(Roles ="admin")]
    [HttpGet]
    public async Task<IActionResult> GetAllOrders()
    {   
        var orders = await _orderService.GetAllOrdersAsync();
        return Ok(orders.Select(o => new {o.Id, o.UserId, o.TotalAmount, o.Status, o.CreatedAt, o.UpdatedAt, 
                o.CanceledAt, o.CanceledReason, 
                OrderItem = o.OrderItems.Select(oi => new {oi.ProductId, oi.Product?.Name, oi.Quantity, oi.Price})}));
    }

    [Authorize(Roles = "user")]
    [HttpGet("user")]
    public async Task<IActionResult> GetUserOrders()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var orders = await _orderService.GetOrderByUserIdAsync(userIdClaim);
        return Ok(orders.Select(o => new {o.Id, o.UserId, o.TotalAmount, o.Status, o.CreatedAt, o.UpdatedAt, 
                o.CanceledAt, o.CanceledReason, 
                OrderItem = o.OrderItems.Select(oi => new {oi.ProductId, oi.Product?.Name, oi.Quantity, oi.Price})}));
    }

    [Authorize]
    [HttpGet("add")]
    public async Task<IActionResult> CreateOrder()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var order = await _orderService.AddOrderAsync(userIdClaim);
        return Ok(order); 
    }

    [Authorize(Roles ="admin")]
    [HttpPut("update")]
    public async Task<IActionResult> UpdateOrder(UpdateOrderStatusRequestDTO updateOrderStatusRequestDTO)
    {
        var order = await _orderService.UpdateOrderStatusAsync(updateOrderStatusRequestDTO.orderId, updateOrderStatusRequestDTO.status);
        return Ok(order); 
    }


    [Authorize]
    [HttpPut("cancel")]
    public async Task<IActionResult> CancelOrder(CancelOrderRequestDTO cancelOrderRequestDTO)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var order = await _orderService.CancelOrderAsync(userIdClaim, cancelOrderRequestDTO.orderId, cancelOrderRequestDTO.CancellationReason);
        return Ok(new {order}); 
    }
}