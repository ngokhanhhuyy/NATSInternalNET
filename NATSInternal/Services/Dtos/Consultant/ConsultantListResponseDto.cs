namespace NATSInternal.Services.Dtos;

public class ConsultantListResponseDto
{
    public int PageCount { get; set; }
    public List<ConsultantBasicResponseDto> Items { get; set; }
    public List<MonthYearResponseDto> MonthYearOptions { get; set; }
    public ConsultantListAuthorizationResponseDto Authorization { get; set; }
}