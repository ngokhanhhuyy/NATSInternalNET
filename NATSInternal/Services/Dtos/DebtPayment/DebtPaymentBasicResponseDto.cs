namespace NATSInternal.Services.Dtos;

public class DebtPaymentBasicResponseDto
{
    public int Id { get; set; }
    public long Amount { get; set; }
    public string Note { get; set; }
    public DateTime PaidDateTime { get; set; }
    public bool IsClosed { get; set; }
    public CustomerBasicResponseDto Customer { get; set; }
    public DebtPaymentAuthorizationResponseDto Authorization { get; set; }
}