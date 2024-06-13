namespace NATSInternal.Services.Dtos;

public class SupplyPhotoRequestDto : IRequestDto<SupplyPhotoRequestDto>
{
    public int? Id { get; set; }
    public byte[] File { get; set; }
    public bool HasBeenChanged { get; set; }

    public SupplyPhotoRequestDto TransformValues()
    {
        Id = Id.HasValue && Id.Value == 0 ? null : Id.Value;
        return this;
    }
}
