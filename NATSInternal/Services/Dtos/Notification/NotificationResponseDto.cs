namespace NATSInternal.Services.Dtos;

public class NotificationResponseDto
{
    public int Id { get; set; }
    public NotificationType Type { get; set; }
    public DateTime DateTime { get; set; }
    public List<int> ResourceIds { get; set; }
    public bool IsRead { get; set; }
    
    public NotificationResponseDto(Notification notification, int currentUserId)
    {
        Id = notification.Id;
        Type = notification.Type;
        DateTime = notification.DateTime;
        ResourceIds = notification.ResourceIds;
        IsRead = notification.ReadUsers.Select(u => u.Id).Contains(currentUserId);
    }
}