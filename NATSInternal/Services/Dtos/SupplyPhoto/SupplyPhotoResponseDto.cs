namespace NATSInternal.Services.Dtos;

public class SupplyPhotoResponseDto
{
    public int Id { get; set; }
    public string Url { get; set; }

    public SupplyPhotoResponseDto(SupplyPhoto photo)
    {
        Id = photo.Id;
        Url = photo.Url;
    }
}
