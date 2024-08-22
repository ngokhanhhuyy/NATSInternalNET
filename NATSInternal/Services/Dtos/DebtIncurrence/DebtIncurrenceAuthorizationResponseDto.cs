namespace NATSInternal.Services.Dtos;

public class DebtIncurrenceAuthorizationResponseDto
{
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
    public bool CanSetCreatedDateTime { get; set; }
}