namespace NATSInternal.Services.Dtos;

public class NotificationListRequestDto : IRequestDto<NotificationListRequestDto>
{
    public bool UnreadOnly { get; set; }
    public int Page { get; set; } = 1;
    public int ResultsPerPage { get; set; } = 15;
    
    public NotificationListRequestDto TransformValues()
    {
        return this;
    }
}