namespace NATSInternal.Services.Dtos;

public class OrderListResponseDto
{
    public int PageCount { get; set; }
    public List<OrderBasicResponseDto> Items { get; set; }
}