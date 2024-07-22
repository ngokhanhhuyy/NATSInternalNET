namespace NATSInternal.Services.Dtos;

public class ExpenseAuthorizationResponseDto
{
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
    public bool CanSetPaidDateTime { get; set; }
}