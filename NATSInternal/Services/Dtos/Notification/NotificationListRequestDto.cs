namespace NATSInternal.Services.Dtos;

public class NotificationListRequestDto : IRequestDto<NotificationListRequestDto>
{
    public bool UnreadOnly { get; set; } = true;
    public int Page { get; set; } = 1;
    public int ResultsPerPage { get; set; } = 5;
    
    public NotificationListRequestDto TransformValues()
    {
        return this;
    }
}