using NATSInternal.Services.Dtos;

namespace NATSInternal.Services.Dtos;

public class OrderPaymentResponseDto
{
    public int Id { get; set; }
    public long Amount { get; set; }
    public DateTime PaidDateTime { get; set; }
    public string Note { get; set; }
    public bool IsClosed { get; set; }
    public UserBasicResponseDto UserInCharge { get; set; }
    public OrderPaymentAuthorizationResponseDto Authorization { get; set; }
}