namespace NATSInternal.Services.Dtos;

public class AnnouncementResponseDto
{
    public int Id { get; set; }
    public AnnouncementCategory Category { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime StartingDateTime { get; set; }
    public DateTime EndingDateTime { get; set; }
    public UserBasicResponseDto User { get; set; }

    public AnnouncementResponseDto(Announcement announcement)
    {
        Id = announcement.Id;
        Category = announcement.Category;
        Title = announcement.Title;
        Content = announcement.Content;
        StartingDateTime = announcement.StartingDateTime;
        EndingDateTime = announcement.EndingDateTime;
        User = new UserBasicResponseDto(announcement.CreatedUser);
    }
}