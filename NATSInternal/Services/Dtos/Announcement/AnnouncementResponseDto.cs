namespace NATSInternal.Services.Dtos;

public class AnnouncementResponseDto
{
    public int Id { get; set; }
    public AnnouncementCategory Category { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime StartingDateTime { get; set; }
    public DateTime EndingDateTime { get; set; }
    public UserWithNamesOnlyResponseDto User { get; set; }
}