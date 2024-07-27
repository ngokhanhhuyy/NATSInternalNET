namespace NATSInternal.Services.Dtos;

public class DebtBasicResponseDto
{
    public int Id { get; set; }
    public long Amount { get; set; }
    public string Note { get; set; }
    public DateTime PaidDateTime { get; set; }
    public bool IsLocked { get; set; }
    public CustomerBasicResponseDto Customer { get; set; }
    public DebtAuthorizationResponseDto Authorization { get; set; }

    public DebtBasicResponseDto(Debt debt)
    {
        MapFromEntity(debt);
    }

    public DebtBasicResponseDto(Debt debt, DebtAuthorizationResponseDto authorization)
    {
        MapFromEntity(debt);
        Authorization = authorization;
    }

    private void MapFromEntity(Debt debt)
    {
        Id = debt.Id;
        Amount = debt.Amount;
        Note = debt.Note;
        PaidDateTime = debt.CreatedDateTime;
        IsLocked = debt.IsLocked;
        Customer = new CustomerBasicResponseDto(debt.Customer);
    }
}
