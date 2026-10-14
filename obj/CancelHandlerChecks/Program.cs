using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OrderAndPaymentAPI.Entities;
using OrderAndPaymentAPI.Exceptions;
using OrderAndPaymentAPI.Services;
using OrderAndPaymentAPI.Web.Pages.Orders;

async Task Check(string name, int id, string? choice, string? text, bool valid, string status = "Pending", bool authenticated = true, bool fail = false)
{
    var service = new FakeOrders { Status = status, Fail = fail };
    var model = new IndexModel(service) { PageContext = new PageContext { HttpContext = new DefaultHttpContext() } };
    if (authenticated)
        model.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "7") }, "test"));
    var result = await model.OnPostCancelOrderAsync(id, choice, text);
    if (valid)
    {
        if (result is not RedirectToPageResult || service.Id != 42 || string.IsNullOrWhiteSpace(service.Reason)) throw new Exception(name);
        if (choice == "other" && service.Reason != text?.Trim()) throw new Exception(name);
    }
    else if (!authenticated)
    {
        if (result is not ChallengeResult || service.Calls != 0) throw new Exception(name);
    }
    else if (result is not PageResult || model.ModelState.IsValid || service.Calls != (fail ? 1 : 0)) throw new Exception(name);
    Console.WriteLine($"PASS: {name}");
}
await Check("preset reason", 42, "change-items", null, true);
await Check("address reason", 42, "change-address", null, true);
await Check("no longer needed", 42, "no-longer-needed", null, true, "Đang xử lý");
await Check("trim custom reason", 42, "other", "  Tôi đặt nhầm  ", true);
await Check("blank custom reason", 42, "other", "   ", false);
await Check("missing reason", 42, null, null, false);
await Check("unknown reason", 42, "forged", null, false);
await Check("too long", 42, "other", new string('a', 501), false);
await Check("invalid id", 0, "change-items", null, false);
await Check("unowned order", 99, "change-items", null, false);
await Check("completed order", 42, "change-items", null, false, "Completed");
await Check("anonymous", 42, "change-items", null, false, authenticated: false);
await Check("service error", 42, "change-items", null, false, fail: true);

class FakeOrders : IOrderService
{
    public string Status = "Pending";
    public bool Fail;
    public int Calls, Id;
    public string? Reason;
    public Task<List<Order>> GetOrderByUserIdAsync(string claim) => Task.FromResult(new List<Order> { new() { Id = 42, UserId = 7, Status = Status } });
    public Task<Order?> CancelOrderAsync(string claim, int id, string reason)
    {
        Calls++; Id = id; Reason = reason;
        if (Fail) throw new AppException("Không thể huỷ đơn.");
        return Task.FromResult<Order?>(new Order { Id = id });
    }
    public Task<Order?> AddOrderAsync(string claim) => throw new NotImplementedException();
    public Task<Order?> UpdateOrderStatusAsync(int id, string status) => throw new NotImplementedException();
}
