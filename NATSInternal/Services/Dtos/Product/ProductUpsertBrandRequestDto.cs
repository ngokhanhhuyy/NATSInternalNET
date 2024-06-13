namespace NATSInternal.Services.Dtos;

public class ProductUpsertBrandRequestDto : IRequestDto<ProductUpsertBrandRequestDto>
{
    public int Id { get; set; }

    public ProductUpsertBrandRequestDto TransformValues()
    {
        return this;
    }
}
