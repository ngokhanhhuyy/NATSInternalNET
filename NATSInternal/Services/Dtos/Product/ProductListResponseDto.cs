namespace NATSInternal.Services.Dtos;

public class ProductListResponseDto
{
    public List<ProductBasicResponseDto> Items { get; set; }
    public int PageCount { get; set; }
    public ProductListAuthorizationResponseDto Authorization { get; set; }
}
