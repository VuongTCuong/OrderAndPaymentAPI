namespace OrderAndPaymentAPI.Web.Models;

public class CartVm
{
    public int Id { get; set; }
    public List<CartItemVm> Items { get; set; } = new();

    public decimal Total => Items.Sum(i => i.Subtotal);
}
