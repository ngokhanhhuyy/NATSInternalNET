namespace NATSInternal.Services.Dtos;

public class ExpenseDetailResponseDto
{
    public int Id { get; set; }
    public long Amount { get; set; }
    public DateTime PaidDateTime { get; set; }
    public ExpenseCategory Category { get; set; }
    public string Note { get; set; }
    public bool IsLocked { get; set; }
    public UserBasicResponseDto User { get; set; }
    public ExpensePayeeResponseDto Payee { get; set; }
    public List<ExpensePhotoResponseDto> Photos { get; set; }
    public ExpenseAuthorizationResponseDto Authorization { get; set; }
    public List<ExpenseUpdateHistoryResponseDto> UpdateHistories { get; set; }

    public ExpenseDetailResponseDto(
            Expense expense,
            ExpenseAuthorizationResponseDto authorization,
            bool mapHistories = false)
    {
        Id = expense.Id;
        Amount = expense.Amount;
        PaidDateTime = expense.PaidDateTime;
        Category = expense.Category;
        Note = expense.Note;
        IsLocked = expense.IsLocked;
        User = new UserBasicResponseDto(expense.CreatedUser);
        Payee = new ExpensePayeeResponseDto(expense.Payee);
        Photos = expense.Photos?.Select(p => new ExpensePhotoResponseDto(p)).ToList();
        Authorization = authorization;
        
        if (mapHistories)
        {
            UpdateHistories = expense.UpdateHistories?
                .Select(uh => new ExpenseUpdateHistoryResponseDto(uh))
                .ToList();
        }
    }
}