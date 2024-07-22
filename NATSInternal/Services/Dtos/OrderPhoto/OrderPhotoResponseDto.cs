namespace NATSInternal.Services.Dtos;

public class OrderPhotoResponseDto
{
    public int Id { get; set; }
    public string Url { get; set; }

    public OrderPhotoResponseDto(OrderPhoto photo)
    {
        Id = photo.Id;
        Url = photo.Url;
    }
}