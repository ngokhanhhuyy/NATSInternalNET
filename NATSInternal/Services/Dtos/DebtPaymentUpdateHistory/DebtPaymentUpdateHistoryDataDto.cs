namespace NATSInternal.Services.Dtos;

public class DebtPaymentUpdateHistoryDataDto
{
    public long Amount { get; set; }
    public string Note { get; set; }
    public DateTime PaidDateTime { get; set; }
    
    public DebtPaymentUpdateHistoryDataDto(DebtPayment payment)
    {
        Amount = payment.Amount;
        Note = payment.Note;
        PaidDateTime = payment.PaidDateTime;
    }
}