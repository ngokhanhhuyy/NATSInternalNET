namespace NATSInternal.Services.Dtos;

public class DebtDetailResponseDto
{
    public int Id { get; set; }
    public long Amount { get; set; }
    public string Note { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public bool IsLocked { get; set; }
    public CustomerBasicResponseDto Customer { get; set; }
    public UserBasicResponseDto User { get; set; }
    public DebtAuthorizationResponseDto Authorization { get; set; }
    public List<DebtUpdateHistoryResponseDto> UpdateHistories { get; set; }

    public DebtDetailResponseDto(
            Debt debt,
            DebtAuthorizationResponseDto authorization,
            bool mapUpdateHistories = false)
    {
        Id = debt.Id;
        Amount = debt.Amount;
        Note = debt.Note;
        CreatedDateTime = debt.CreatedDateTime;
        IsLocked = debt.IsLocked;
        Customer = new CustomerBasicResponseDto(debt.Customer);
        User = new UserBasicResponseDto(debt.CreatedUser);
        Authorization = authorization;
        
        if (mapUpdateHistories)
        {
            UpdateHistories = debt.UpdateHistories
                .Select(uh => new DebtUpdateHistoryResponseDto(uh))
                .ToList();
        }
    }
}
