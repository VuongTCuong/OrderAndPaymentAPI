

using OrderAndPaymentAPI.Entities;

public class RefreshToken
{
    public int Id {get; set;}
    public string Token {get; set;} = String.Empty;

    public int UserId {get; set;}   
    public User User {get; set; } = null!;
    public DateTime ExpiredDate {get; set;}
    public bool IsRevoked {get; set;}

}