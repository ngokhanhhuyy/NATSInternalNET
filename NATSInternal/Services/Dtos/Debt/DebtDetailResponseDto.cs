namespace NATSInternal.Services.Dtos;

public class DebtDetailResponseDto
{
    public int Id { get; set; }
    public long Amount { get; set; }
    public string Note { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public bool IsClosed { get; set; }
    public CustomerBasicResponseDto Customer { get; set; }
    public UserBasicResponseDto User { get; set; }
    public DebtAuthorizationResponseDto Authorization { get; set; }

    public DebtDetailResponseDto(Debt debt, DebtAuthorizationResponseDto authorization)
    {
        Id = debt.Id;
        Amount = debt.Amount;
        Note = debt.Note;
        CreatedDateTime = debt.CreatedDateTime;
        IsClosed = debt.IsClosed;
        Customer = new CustomerBasicResponseDto(debt.Customer);
        User = new UserBasicResponseDto(debt.CreatedUser);
        Authorization = authorization;
    }
}
