
namespace OrderAndPaymentAPI.DTO;

public class UpdateOrderStatusRequestDTO
{
    public int orderId {get; set;}
    public string? status {get;set;}
}