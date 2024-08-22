namespace NATSInternal.Services.Dtos;

public class DebtIncurrenceUpdateHistoryDataDto
{
    public long Amount { get; set; }
    public string Note { get; set; }
    public DateTime IncurredDateTime { get; set; }
    
    public DebtIncurrenceUpdateHistoryDataDto(DebtIncurrence debt)
    {
        Amount = debt.Amount;
        Note = debt.Note;
        IncurredDateTime = debt.IncurredDateTime;
    }
}