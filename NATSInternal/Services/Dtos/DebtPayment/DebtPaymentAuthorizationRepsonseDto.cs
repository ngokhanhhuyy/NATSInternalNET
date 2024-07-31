namespace NATSInternal.Services.Dtos;

public class DebtPaymentAuthorizationResponseDto
{
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
    public bool CanSetPaidDateTime { get; set; }
}
