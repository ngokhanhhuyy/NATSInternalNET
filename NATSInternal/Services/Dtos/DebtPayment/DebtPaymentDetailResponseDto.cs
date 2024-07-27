namespace NATSInternal.Services.Dtos;

public class DebtPaymentDetailResponseDto
{
    public int Id { get; set; }
    public long Amount { get; set; }
    public string Note { get; set; }
    public DateTime PaidDateTime { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public bool IsLocked { get; set; }
    public CustomerBasicResponseDto Customer { get; set; }
    public UserBasicResponseDto User { get; set; }
    public DebtPaymentAuthorizationResponseDto Authorization { get; set; }
    public List<DebtPaymentUpdateHistoryResponseDto> UpdateHistories { get; set; }

    public DebtPaymentDetailResponseDto(
            DebtPayment payment,
            DebtPaymentAuthorizationResponseDto authorization,
            bool mapUpdateHistories = false)
    {
        Id = payment.Id;
        Amount = payment.Amount;
        Note = payment.Note;
        PaidDateTime = payment.PaidDateTime;
        CreatedDateTime = payment.CreatedDateTime;
        IsLocked = payment.IsLocked;
        Customer = new CustomerBasicResponseDto(payment.Customer);
        User = new UserBasicResponseDto(payment.CreatedUser);
        Authorization = authorization;
        
        if (mapUpdateHistories)
        {
            UpdateHistories = payment.UpdateHistories
                .Select(uh => new DebtPaymentUpdateHistoryResponseDto(uh))
                .ToList();
        }
    }
}