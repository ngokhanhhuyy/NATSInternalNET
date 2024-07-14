namespace NATSInternal.Services.Dtos;

public class DebtBasicResponseDto
{
    public int Id { get; set; }
    public long Amount { get; set; }
    public string Note { get; set; }
    public bool IsClosed { get; set; }
    public CustomerBasicResponseDto Customer { get; set; }
    public DebtAuthorizationResponseDto Authorization { get; set; }
}
