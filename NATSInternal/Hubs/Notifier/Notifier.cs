namespace NATSInternal.Hubs.Notifier;

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