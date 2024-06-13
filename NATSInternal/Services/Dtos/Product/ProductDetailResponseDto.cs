namespace NATSInternal.Services.Dtos;

public class ProductDetailResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Unit { get; set; }
    public long Price { get; set; }
    public decimal VatFactor { get; set; }
    public bool IsForRetail { get; set; }
    public bool IsDiscontinued { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime? UpdatedDateTime { get; set; }
    public string ThumbnailUrl { get; set; }
    public ProductCategoryResponseDto Category { get; set; }
    public BrandBasicResponseDto Brand { get; set; }
}
