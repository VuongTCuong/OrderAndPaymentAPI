namespace OrderAndPaymentAPI.DTO;

public class CancelOrderRequestDTO
{
    public int orderId { get; set; }
    public string CancellationReason { get; set; } = string.Empty;
}
