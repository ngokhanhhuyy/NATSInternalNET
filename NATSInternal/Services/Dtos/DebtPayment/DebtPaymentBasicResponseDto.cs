namespace NATSInternal.Services.Dtos;

public class DebtPaymentBasicResponseDto
{
    public int Id { get; set; }
    public long Amount { get; set; }
    public string Note { get; set; }
    public DateTime PaidDateTime { get; set; }
    public bool IsLocked { get; set; }
    public CustomerBasicResponseDto Customer { get; set; }
    public DebtPaymentAuthorizationResponseDto Authorization { get; set; }

    public DebtPaymentBasicResponseDto(DebtPayment payment)
    {
        MapFromEntity(payment);
    }

    public DebtPaymentBasicResponseDto(
            DebtPayment payment,
            DebtPaymentAuthorizationResponseDto authorization)
    {
        MapFromEntity(payment);
        Authorization = authorization;
    }

    private void MapFromEntity(DebtPayment payment)
    {
        Id = payment.Id;
        Amount = payment.Amount;
        Note = payment.Note;
        PaidDateTime = payment.PaidDateTime;
        IsLocked = payment.IsLocked;
        Customer = new CustomerBasicResponseDto(payment.Customer);
    }
}