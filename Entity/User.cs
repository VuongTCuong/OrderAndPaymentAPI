namespace OrderAndPaymentAPI.Entities;
public class User
{
    public int Id {get; set;}
    public string Email {get; set;} = String.Empty;
    public string Password {get; set;} = String.Empty;
    public string Fullname {get; set;} = String.Empty;
    public string Role {get; set;} = String.Empty;
    public DateTime CreatedDate {get; set;}

    // 1 user - 1 cart
    public Cart? Cart {get; set;} 

    // 1 user - N order
    public ICollection<Order> Orders {get; set;} = new List<Order>();
}