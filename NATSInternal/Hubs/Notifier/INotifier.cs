namespace NATSInternal.Hubs.Notifier;

/// <summary>
/// A class to send notification to users who are connecting to the
/// <c><see cref="ApplicationHub" /></c>.
/// </summary>
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