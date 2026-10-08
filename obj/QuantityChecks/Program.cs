using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OrderAndPaymentAPI.Entities;
using OrderAndPaymentAPI.Exceptions;
using OrderAndPaymentAPI.Services;
using OrderAndPaymentAPI.Web.Pages.Products;

var cart = DispatchProxy.Create<ICartService, ServiceProxy>();
var products = DispatchProxy.Create<IProductService, ServiceProxy>();
var calls = 0;
var fail = false;
((ServiceProxy)(object)products).Handler = (_, _) => Task.FromResult(new List<Product>());
((ServiceProxy)(object)cart).Handler = (method, args) => {
    if (method.Name != "AddItemAsync") throw new Exception("Unexpected call");
    calls++;
    if ((string)args![0]! != "7" || (int)args[1]! != 42) throw new Exception("Wrong product or user");
    if ((int)args[2]! <= 0) return Task.FromException<Cart>(new AppException("Số lượng phải lớn hơn 0."));
    if ((int)args[2]! != 3) throw new Exception("Wrong quantity");
    return fail ? Task.FromException<Cart>(new AppException("Số lượng vượt quá tồn kho.", 409)) : Task.FromResult(new Cart());
};
IndexModel Model(bool authenticated = true) => new(products, cart) {
    PageContext = new PageContext { HttpContext = new DefaultHttpContext { User = authenticated
        ? new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "7") }, "test"))
        : new ClaimsPrincipal(new ClaimsIdentity()) } }
};
void Check(bool ok) { if (!ok) throw new Exception("Check failed"); }
Check(await Model(false).OnPostAsync(42, 3) is RedirectToPageResult { PageName: "/Account/Login" });
Check(calls == 0);
Check(await Model().OnPostAsync(42, 3) is RedirectToPageResult { PageName: "/CartPage/Index" });
Check(calls == 1);
foreach (var quantity in new[] { 0, -1 }) {
    var model = Model();
    Check(await model.OnPostAsync(42, quantity) is PageResult && !model.ModelState.IsValid);
}
Check(calls == 3);
var invalid = Model();
invalid.ModelState.AddModelError("quantity", "Invalid number");
Check(await invalid.OnPostAsync(42, 3) is PageResult && calls == 3);
fail = true;
var exceeded = Model();
Check(await exceeded.OnPostAsync(42, 3) is PageResult && !exceeded.ModelState.IsValid);
Console.WriteLine("PASS: anonymous redirect, selected quantity, zero/negative, binding error, stock error.");
var updates = 0;
var stockError = false;
((ServiceProxy)(object)cart).Handler = (method, args) => {
    if (method.Name == "GetCartByUserIdAsync") return Task.FromResult<Cart?>(null);
    if (method.Name != "UpdateItemQuantityAsync") throw new Exception("Unexpected call");
    updates++;
    Check((int)args![0]! == 7 && (int)args[1]! == 42 && (int)args[2]! == 3);
    return stockError ? Task.FromException<Cart>(new AppException("Số lượng vượt quá tồn kho.", 409)) : Task.FromResult(new Cart());
};
OrderAndPaymentAPI.Web.Pages.CartPage.IndexModel CartModel(bool authenticated = true) => new(cart) {
    PageContext = Model(authenticated).PageContext
};
Check(await CartModel(false).OnPostUpdateQuantityAsync(42, 3) is RedirectToPageResult { PageName: "/Account/Login" });
Check(updates == 0);
Check(await CartModel().OnPostUpdateQuantityAsync(42, 3) is RedirectToPageResult);
Check(updates == 1);
var invalidUpdate = CartModel();
invalidUpdate.ModelState.AddModelError("quantity", "Invalid number");
Check(await invalidUpdate.OnPostUpdateQuantityAsync(42, 3) is PageResult && updates == 1);
Check(invalidUpdate.cart.CartItems.Count == 0);
stockError = true;
var rejectedUpdate = CartModel();
Check(await rejectedUpdate.OnPostUpdateQuantityAsync(42, 3) is PageResult && !rejectedUpdate.ModelState.IsValid);
Console.WriteLine("PASS: cart update uses signed-in user and selected quantity; anonymous redirect; binding and service errors; missing cart.");
public class ServiceProxy : DispatchProxy {
    public Func<MethodInfo, object?[]?, object?> Handler = null!;
    protected override object? Invoke(MethodInfo? method, object?[]? args) => Handler(method!, args);
}
