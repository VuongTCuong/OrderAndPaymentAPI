namespace OrderAndPaymentAPI.Entities;

public class Order
{
    public int Id {get; set;}
    public int UserId {get; set;}
    public User User {get; set;} = null!;
    public decimal TotalAmount {get; set;}
    public string Status {get; set;} = String.Empty;
    public DateTime CreatedAt {get; set;}
    public DateTime UpdatedAt {get; set;}

    // 1 order - N orderitem
    public ICollection<OrderItem> OrderItems = new List<OrderItem>();
}