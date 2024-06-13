namespace NATSInternal.Services.Dtos;

public class SupplyListResponseDto
{
    public List<SupplyBasicResponseDto> Items { get; set; }
    public int PageCount { get; set; }
}
