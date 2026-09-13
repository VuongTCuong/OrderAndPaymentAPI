
namespace OrderAndPaymentAPI.DTO;
public class ProductDTO
{
    public string Name {get; set;} = String.Empty;
    public decimal Price {get; set;}
    public int Stock {get; set;}
}