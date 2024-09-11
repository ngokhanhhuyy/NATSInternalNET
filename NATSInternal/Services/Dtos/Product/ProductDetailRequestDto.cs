namespace NATSInternal.Services.Dtos;

public class ProductDetailRequestDto : IRequestDto<ProductDetailRequestDto>
{
    public int RecentSuppliesResultCount { get; set; } = 5;
    public int RecentOrdersResultCount { get; set; } = 5;
    public int RecentTreatmentsResultCount { get; set; } = 5;

    public ProductDetailRequestDto TransformValues()
    {
        return this;
    }
}