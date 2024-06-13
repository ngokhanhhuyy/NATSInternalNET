using NATSInternal.Services.Dtos;

namespace NATSInternal.Services.Dtos;

public class ProductCategoryListResponseDto
{
    public List<ProductCategoryResponseDto> Items { get; set; }
    public ProductCategoryAuthorizationResponseDto Authorization { get; set; }
}
