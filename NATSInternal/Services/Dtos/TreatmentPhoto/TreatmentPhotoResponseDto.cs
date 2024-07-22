namespace NATSInternal.Services.Dtos;

public class TreatmentPhotoResponseDto
{
    public int Id { get; set; }
    public string Url { get; set; }
    public TreatmentPhotoType Type { get; set; }

    public TreatmentPhotoResponseDto(TreatmentPhoto treatmentPhoto)
    {
        Id = treatmentPhoto.Id;
        Url = treatmentPhoto.Url;
        Type = treatmentPhoto.Type;
    }
}