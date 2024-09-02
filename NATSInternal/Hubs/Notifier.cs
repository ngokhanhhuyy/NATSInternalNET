namespace NATSInternal.Hubs;

public interface INotifier
{
    /// <summary>
    /// Create a notification with the specified type and distribute it to the users
    /// who have been specifed to be the receivers of the notification and are
    /// connecting to the notification hub.
    /// </summary>
    /// <param name="notificationType">
    /// The type of the notification.
    /// </param>
    /// <param name="resourceIds">
    /// (Optional) The ids of the resource if the notification type is to indicate
    /// that some resource has been interacted.
    /// </param>
    /// <returns>A <c>Task</c> object representing the asynchronous operation.</returns>
    Task Notify(NotificationType notificationType, params int[] resourceIds);
}

/// <inheritdoc cref="INotifier" />
public class Notifier : INotifier
{
    private readonly IHubContext<ApplicationHub> _hubContext;
    private readonly INotificationService _notificationService;
    
    public Notifier(
            IHubContext<ApplicationHub> hubContext,
            INotificationService notificationService)
    {
        _hubContext = hubContext;
        _notificationService = notificationService;
    }
    
    /// <inheritdoc />
    public async Task Notify(
            NotificationType notificationType, params int[] resourceIds)
    {
        // Create the notification.
        List<int> userIds;
        int notificationId;
        (userIds, notificationId) = await _notificationService.CreateAsync(
            notificationType,
            resourceIds.ToList());
        
        // Get the created notification data.
        NotificationResponseDto responseDto = await _notificationService
            .GetSingleAsync(notificationId);
        
        // Distribute the notification to the users.
        foreach (int userId in userIds)
        {
            await _hubContext.Clients
                .User(userId.ToString())
                .SendAsync("NotificationDistributed", responseDto);
        }
    }
}