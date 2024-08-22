namespace NATSInternal.Services.Dtos;

public class NotificationListRequestDto : IRequestDto<NotificationListRequestDto>
{
    public int Page { get; set; } = 1;
    public int ResultsPerPage { get; set; } = 15;
    
    public NotificationListRequestDto TransformValues()
    {
        return this;
    }
}