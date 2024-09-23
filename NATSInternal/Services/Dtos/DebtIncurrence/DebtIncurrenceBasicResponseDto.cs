namespace NATSInternal.Services.Dtos;

public class DebtIncurrenceBasicResponseDto
{
    public int Id { get; set; }
    public long Amount { get; set; }
    public string Note { get; set; }
    public DateTime IncurredDateTime { get; set; }
    public bool IsLocked { get; set; }
    public CustomerBasicResponseDto Customer { get; set; }
    public DebtIncurrenceAuthorizationResponseDto Authorization { get; set; }

    public DebtIncurrenceBasicResponseDto(DebtIncurrence debt)
    {
        MapFromEntity(debt);
    }

    public DebtIncurrenceBasicResponseDto(
            DebtIncurrence debt,
            DebtIncurrenceAuthorizationResponseDto authorization)
    {
        MapFromEntity(debt);
        Authorization = authorization;
    }

    private void MapFromEntity(DebtIncurrence debt)
    {
        Id = debt.Id;
        Amount = debt.Amount;
        Note = debt.Note;
        IncurredDateTime = debt.CreatedDateTime;
        IsLocked = debt.IsLocked;
        Customer = new CustomerBasicResponseDto(debt.Customer);
    }
}
