using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderAndPaymentAPI.Controllers;
using OrderAndPaymentAPI.DTO;
using OrderAndPaymentAPI.Entities;
using OrderAndPaymentAPI.Services;

var service = DispatchProxy.Create<ICartService, CartProxy>();
var proxy = (CartProxy)(object)service;
var controller = new CartController(service) {
    ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext {
        User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "42"), new Claim(ClaimTypes.Role, "user") }, "test"))
    }}
};
void Check(bool condition) { if (!condition) throw new Exception("Check failed"); }
var result = await controller.UpdateCart(new UpdateCartRequestDTO { productId = 7, quantity = 3 });
Check(proxy.Method == "UpdateItemQuantityAsync" && (string)proxy.Args[0] == "42" && (int)proxy.Args[1] == 7 && (int)proxy.Args[2] == 3);
var json = System.Text.Json.JsonSerializer.Serialize(((OkObjectResult)result).Value);
Check(json.Contains("\"ProductId\":7") && json.Contains("\"Quantity\":3") && json.Contains("\"ProductName\":\"Test\"") && json.Contains("\"Price\":12"));
Check(await controller.DeleteItem(7) is OkObjectResult);
Check(proxy.Method == "DeleteItemAsync" && (string)proxy.Args[0] == "42" && (int)proxy.Args[1] == 7);
Check(await controller.ClearCart() is OkObjectResult);
Check(proxy.Method == "ClearCartAsync" && (string)proxy.Args[0] == "42");
foreach (var name in new[] { "UpdateCart", "DeleteItem", "ClearCart" }) Check(typeof(CartController).GetMethod(name).GetCustomAttribute<AuthorizeAttribute>().Roles == "user");
Check(typeof(CartController).GetMethod("UpdateCart").GetCustomAttribute<HttpPutAttribute>().Template == "update");
Check(typeof(CartController).GetMethod("DeleteItem").GetCustomAttribute<HttpDeleteAttribute>().Template == "delete/{productId:int}");
Check(typeof(CartController).GetMethod("ClearCart").GetCustomAttribute<HttpDeleteAttribute>().Template == "clear");
Console.WriteLine("PASS: three cart actions, claim forwarding, update response, route attributes and user role.");
public class CartProxy : DispatchProxy {
    public string Method; public object[] Args;
    protected override object Invoke(MethodInfo method, object[] args) {
        Method = method.Name; Args = args;
        if (method.Name == "UpdateItemQuantityAsync") return Task.FromResult(new Cart { CartItems = new List<CartItem> { new CartItem { CartId = 1, ProductId = 7, Quantity = 3, Product = new Product { Name = "Test", Price = 12 } } } });
        return Task.CompletedTask;
    }
}
