namespace NATSInternal.Services.Dtos;

public class ExpenseBasicResponseDto
{
    public int Id { get; set; }
    public long Amount { get; set; }
    public DateTime PaidDateTime { get; set; }
    public ExpenseCategory Category { get; set; }
    public bool IsClosed { get; set; }
    public ExpenseAuthorizationResponseDto Authorization { get; set; }

    public ExpenseBasicResponseDto(Expense expense)
    {
        MapFromEntity(expense);
    }

    public ExpenseBasicResponseDto(
            Expense expense,
            ExpenseAuthorizationResponseDto authorization)
    {
        MapFromEntity(expense);
        Authorization = authorization;
    }

    private void MapFromEntity(Expense expense)
    {
        Id = expense.Id;
        Amount = expense.Amount;
        PaidDateTime = expense.PaidDateTime;
        Category = expense.Category;
        IsClosed = expense.IsClosed;
    }
}