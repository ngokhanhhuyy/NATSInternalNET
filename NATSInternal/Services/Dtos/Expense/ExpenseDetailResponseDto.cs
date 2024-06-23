namespace NATSInternal.Services.Dtos;

public class ExpenseDetailResponseDto
{
    public int Id { get; set; }
    public long Amount { get; set; }
    public DateTime PaidDateTime { get; set; }
    public ExpenseCategory Category { get; set; }
    public string Note { get; set; }
    public UserBasicResponseDto User { get; set; }
    public ExpensePayeeResponseDto Payee { get; set; }
    public List<ExpensePhotoResponseDto> Photos { get; set; }
    public ExpenseAuthorizationResponseDto Authorization { get; set; }
}