namespace OrderAndPaymentAPI.Entities;

public class Cart
{
    public int Id {get; set;}
    public DateTime CreatedAt {get; set;}
    public DateTime UpdatedAt {get; set;}
    public int UserId {get; set;}
    public User User{get; set;} = null!;

    // 1 cart - N cartitem
    public ICollection<CartItem> CartItems{get; set;} = new List<CartItem>();
}