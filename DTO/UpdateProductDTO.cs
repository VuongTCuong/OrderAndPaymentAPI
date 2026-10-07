namespace OrderAndPaymentAPI.DTO;
public class UpdateProductDTO{
    public int Id {get; set;}
    public string Name {get; set;} = String.Empty;
    public decimal Price {get; set;} 
    public int Stock {get; set;}

    
}