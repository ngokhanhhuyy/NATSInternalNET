namespace NATSInternal.Services.Dtos;

public class TreatmentPhotoRequestDto : IRequestDto<TreatmentPhotoRequestDto>
{
    public int? Id { get; set; }
    public byte[] File { get; set; }
    public bool HasBeenChanged { get; set; }

    public TreatmentPhotoRequestDto TransformValues()
    {
        return this;
    }
}
