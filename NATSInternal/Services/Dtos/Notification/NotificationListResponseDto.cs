namespace NATSInternal.Services.Dtos;

public class NotificationListResponseDto
{
    public int PageCount { get; set; }
    public List<NotificationResponseDto> Items { get; set; }
}