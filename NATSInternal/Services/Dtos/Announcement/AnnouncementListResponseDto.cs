namespace NATSInternal.Services.Dtos;

public class AnnouncementListResponseDto
{
    public List<AnnouncementResponseDto> Items { get; set; }
    public int PageCount { get; set; }
}