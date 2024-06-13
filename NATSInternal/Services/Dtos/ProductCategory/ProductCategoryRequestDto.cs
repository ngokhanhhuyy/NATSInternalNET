namespace NATSInternal.Services.Dtos;

public class ProductCategoryRequestDto : IRequestDto<ProductCategoryRequestDto>
{
    public int Id { get; set; }
    public string Name { get; set; }

    public ProductCategoryRequestDto TransformValues()
    {
        Name = Name?.ToNullIfEmpty();
        return this;
    }
}