namespace OrderAndPaymentAPI.Web.Models;

public class OrderVm
{
    public int Id { get; set; }
    public string Status { get; set; } = string.Empty; // Pending, Completed, Cancelled
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<OrderItemVm> Items { get; set; } = new();
}
