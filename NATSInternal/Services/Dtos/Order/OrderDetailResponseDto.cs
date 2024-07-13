namespace NATSInternal.Services.Dtos;

public class OrderDetailResponseDto
{
    public int Id { get; set; }
    public DateTime OrderedDateTime { get; set; }
    public long Amount { get; set; }
    public string Note { get; set; }
    public bool IsClosed { get; set; }
    public List<OrderItemResponseDto> Items { get; set; }
    public List<DebtPaymentResponseDto> Payments { get; set; }
    public CustomerBasicResponseDto Customer { get; set; }
    public UserBasicResponseDto User { get; set; }
    public List<OrderPhotoResponseDto> Photos { get; set; }
    public OrderAuthorizationResponseDto Authorization { get; set; }
}