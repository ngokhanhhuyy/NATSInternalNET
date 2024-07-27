namespace NATSInternal.Services.Dtos;

public class DebtUpdateHistoryDataDto
{
    public long Amount { get; set; }
    public string Note { get; set; }
    public DateTime IncurredDateTime { get; set; }
    
    public DebtUpdateHistoryDataDto(Debt debt)
    {
        Amount = debt.Amount;
        Note = debt.Note;
        IncurredDateTime = debt.IncurredDateTime;
    }
}