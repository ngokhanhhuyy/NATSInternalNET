namespace NATSInternal.Services.Dtos;

public class ProductUpsertRequestDto
        : IRequestDto<ProductUpsertRequestDto>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Unit { get; set; }
    public long Price { get; set; }
    public decimal VatFactor { get; set; }
    public bool IsForRetail { get; set; }
    public bool IsDiscontinued { get; set; }
    public byte[] ThumbnailFile { get; set; }
    public bool ThumbnailChanged { get; set; }
    public int CategoryId { get; set; }
    public int BrandId { get; set; }
    public List<ProductPhotoRequestDto> Photos { get; set; }

    public ProductUpsertRequestDto TransformValues()
    {
        Name = Name?.ToNullIfEmpty();
        Description = Description?.ToNullIfEmpty();
        Unit = Unit?.ToNullIfEmpty();
        return this;
    }
}