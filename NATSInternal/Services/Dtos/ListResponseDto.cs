namespace NATSInternal.Services.Dtos;

public class ListResponseDto<TBasicResponseDto>
{
    public int PageCount { get; set; }
    public List<TBasicResponseDto> Items { get; set; }
}
