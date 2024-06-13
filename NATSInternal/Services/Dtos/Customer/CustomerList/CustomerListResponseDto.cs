namespace NATSInternal.Services.Dtos;

public class CustomerListResponseDto
{
    public int PageCount { get; set; }
    public List<CustomerBasicResponseDto> Results { get; set; }
}