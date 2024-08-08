namespace NATSInternal.Services.Dtos;

public class LockableEntityListResponseDto<TBasicResponseDto> : ListResponseDto<TBasicResponseDto>
{
    public List<MonthYearResponseDto> MonthYearOptions { get; set; }
}
