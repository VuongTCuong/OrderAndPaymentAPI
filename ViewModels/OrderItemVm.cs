namespace OrderAndPaymentAPI.Web.Models;

public class OrderItemVm
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }

    public decimal Subtotal => Price * Quantity;
}
