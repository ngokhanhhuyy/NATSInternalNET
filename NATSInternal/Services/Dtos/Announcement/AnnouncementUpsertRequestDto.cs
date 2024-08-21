namespace NATSInternal.Services.Dtos;

public class AnnouncementUpsertRequestDto : IRequestDto<AnnouncementUpsertRequestDto>
{
    public AnnouncementCategory Category { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime StartingDateTime { get; set; } = DateTime.UtcNow.ToApplicationTime();
    public int IntervalInMinutes { get; set; } = 24 * 60;

    public AnnouncementUpsertRequestDto TransformValues()
    {
        Title = Title?.ToNullIfEmpty();
        Content = Content?.ToNullIfEmpty();
        return this;
    }
}