namespace NATSInternal.Services.Dtos;

public class DebtPaymentDetailResponseDto
{
    public int Id { get; set; }
    public long Amount { get; set; }
    public string Note { get; set; }
    public DateTime PaidDateTime { get; set; }
    public bool IsClosed { get; set; }
    public CustomerBasicResponseDto Customer { get; set; }
    public UserBasicResponseDto User { get; set; }
    public DebtPaymentAuthorizationResponseDto Authorization { get; set; }

    public DebtPaymentDetailResponseDto(
            DebtPayment payment,
            DebtPaymentAuthorizationResponseDto authorization)
    {
        Id = payment.Id;
        Amount = payment.Amount;
        Note = payment.Note;
        PaidDateTime = payment.PaidDateTime;
        IsClosed = payment.IsClosed;
        Customer = new CustomerBasicResponseDto(payment.Customer);
        User = new UserBasicResponseDto(payment.CreatedUser);
        Authorization = authorization;
    }
}