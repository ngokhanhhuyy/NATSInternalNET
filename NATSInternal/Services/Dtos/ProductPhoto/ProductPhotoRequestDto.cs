namespace NATSInternal.Services.Dtos;

public class ProductPhotoRequestDto : IRequestDto<ProductPhotoRequestDto>
{
    public int? Id { get; set; }
    public byte[] File { get; set; }
    public bool HasBeenChanged { get; set; }

    public ProductPhotoRequestDto TransformValues()
    {
        Id = Id.HasValue && Id.Value == 0 ? null : Id;
        return this;
    }
}