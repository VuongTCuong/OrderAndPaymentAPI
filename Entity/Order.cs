namespace OrderAndPaymentAPI.Entities;

public class Order
{
    public int Id {get; set;}
    public int UserId {get; set;}
    public User? User {get; set;} 
    public decimal TotalAmount {get; set;}
    public string Status {get; set;} = String.Empty;
    public DateTime CreatedAt {get; set;}
    public DateTime UpdatedAt {get; set;}

    public DateTime? CanceledAt {get;set;}
    public string? CanceledReason {get;set;}

    // 1 order - N orderitem
    public ICollection<OrderItem> OrderItems{get;set;} = new List<OrderItem>();
}