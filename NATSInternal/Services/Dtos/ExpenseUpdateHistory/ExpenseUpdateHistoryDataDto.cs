namespace NATSInternal.Services.Dtos;

public class ExpenseUpdateHistoryDataDto
{
    public long Amount { get; set; }
    public DateTime PaidDateTime { get; set; }
    public ExpenseCategory Category { get; set; }
    public string Note { get; set; }
    public string PayeeName { get; set; }
    
    public ExpenseUpdateHistoryDataDto(Expense expense)
    {
        Amount = expense.Amount;
        PaidDateTime = expense.PaidDateTime;
        Category = expense.Category;
        Note = expense.Note;
        PayeeName = expense.Payee.Name;
    }
}