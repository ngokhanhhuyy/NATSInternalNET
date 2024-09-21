namespace NATSInternal.Services.Dtos;

public class DebtIncurrenceDetailResponseDto
{
    public int Id { get; set; }
    public long Amount { get; set; }
    public string Note { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public bool IsLocked { get; set; }
    public CustomerBasicResponseDto Customer { get; set; }
    public UserBasicResponseDto CreatedUser { get; set; }
    public DebtIncurrenceAuthorizationResponseDto Authorization { get; set; }
    public List<DebtIncurrenceUpdateHistoryResponseDto> UpdateHistories { get; set; }

    public DebtIncurrenceDetailResponseDto(
            DebtIncurrence debt,
            DebtIncurrenceAuthorizationResponseDto authorization,
            bool mapUpdateHistories = false)
    {
        Id = debt.Id;
        Amount = debt.Amount;
        Note = debt.Note;
        CreatedDateTime = debt.CreatedDateTime;
        IsLocked = debt.IsLocked;
        Customer = new CustomerBasicResponseDto(debt.Customer);
        CreatedUser = new UserBasicResponseDto(debt.CreatedUser);
        Authorization = authorization;
        
        if (mapUpdateHistories)
        {
            UpdateHistories = debt.UpdateHistories
                .Select(uh => new DebtIncurrenceUpdateHistoryResponseDto(uh))
                .ToList();
        }
    }
}
