namespace NATSInternal.Services.Dtos;

public class ProductListRequestDto : IRequestDto<ProductListRequestDto>
{
    public string CategoryName { get; set; }
    public int? BrandId { get; set; }
    public string ProductName { get; set; }
    public int Page { get; set; } = 0;
    public int ResultsPerPage { get; set; } = 15;

    public ProductListRequestDto TransformValues()
    {
        CategoryName = CategoryName?.ToNullIfEmpty();
        ProductName = ProductName?.ToNullIfEmpty();
        return this;
    }
}
