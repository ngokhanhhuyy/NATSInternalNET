namespace NATSInternal.Services.Dtos;

public class ExpensePhotoResponseDto
{
    public int Id { get; set; }
    public string Url { get; set; }

    public ExpensePhotoResponseDto(ExpensePhoto photo)
    {
        Id = photo.Id;
        Url = photo.Url;
    }
}