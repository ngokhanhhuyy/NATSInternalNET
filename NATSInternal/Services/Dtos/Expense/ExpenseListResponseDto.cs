namespace NATSInternal.Services.Dtos;

public class ExpenseListResponseDto
{
    public List<ExpenseBasicResponseDto> Items { get; set; }
    public int PageCount { get; set; }
    public List<MonthYearResponseDto> MonthYearOptions { get; set; }
    public ExpenseListAuthorizationResponseDto Authorization { get; set; }
}