namespace NATSInternal.Services.Dtos;

public class SupplyListResponseDto
{
    public List<SupplyBasicResponseDto> Items { get; set; }
    public int PageCount { get; set; }
    public List<MonthYearResponseDto> MonthYearOptions { get; set; }
    public SupplyListAuthorizationResponseDto Authorization { get; set; }
}
