namespace NATSInternal.Services.Dtos;

public class ExpenseBasicResponseDto
{
    public int Id { get; set; }
    public long Amount { get; set; }
    public DateTime PaidDateTime { get; set; }
    public ExpenseCategory Category { get; set; }
    public bool IsClosed { get; set; }
    public ExpenseAuthorizationResponseDto Authorization { get; set; }
}