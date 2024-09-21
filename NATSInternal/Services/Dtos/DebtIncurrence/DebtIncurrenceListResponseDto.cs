namespace NATSInternal.Services.Dtos;

public class DebtIncurrenceListResponseDto
{
    public List<DebtIncurrenceBasicResponseDto> Items { get; set; }
    public int PageCount { get; set; }
    public List<MonthYearResponseDto> MonthYearOptions { get; set; }
    public DebtIncurrenceListAuthorizationResponseDto Authorization { get; set; }
}