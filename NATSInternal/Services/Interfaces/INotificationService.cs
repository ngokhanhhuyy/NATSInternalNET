namespace NATSInternal.Services.Interfaces;

/// <summary>
/// A service to handle notifications.
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Retrieves a list of notifications, based on the filtering and paginating conditions
    /// (if specified).
    /// </summary>
    /// <param name="requestDto">
    /// (Optional) An instance of the <see cref="NotificationListRequestDto"/> class,
    /// containing the conditions for the results.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation, which result is an
    /// instance of the <see cref="NotificationListRequestDto"/> class, containing the results
    /// and the additional information for pagination.
    /// </returns>
    Task<NotificationListResponseDto> GetListAsync(
            NotificationListRequestDto requestDto = null);

    /// <summary>
    /// Retrieves the information of a single specific notification, based on its id.
    /// </summary>
    /// <param name="id">
    /// An <see cref="int"/> representing the id of the notification to retrieve.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation, which result is an
    /// instance of the <see cref="NotificationResponseDto"/> class, containing the information
    /// of the notification.
    /// </returns>
    /// <exception cref="ResourceNotFoundException">
    /// Throws when the notification with the specified id cannot be found.
    /// </exception>
    Task<NotificationResponseDto> GetSingleAsync(int id);

    /// <summary>
    /// Marks a specific notification as read, based on its id.
    /// </summary>
    /// <param name="id">
    /// An <see cref="int"/> representing the id of the notification to mark.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    /// <exception cref="ResourceNotFoundException">
    /// Throws when the notification with the specified id which belongs to the requesting user
    /// cannot be found.
    /// </exception>
    Task MarkAsReadAsync(int id);

    /// <summary>
    /// Marks all the notifications which belong to the requesting user as read.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    Task MarkAllAsReadAsync();

    /// <summary>
    /// Creates a notification which all users can receive with the specified notification type
    /// and resource ids.
    /// </summary>
    /// <param name="type">
    /// An element of the <see cref="NotificationType"/> enumeration, representing the type of
    /// the notification to create.
    /// </param>
    /// <param name="resourceIds">
    /// A <see cref="List{T}"/> where <c>T</c> is <see cref="int"/>, representing the ids of
    /// the resource that has been interacted.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation, which result is a
    /// <see cref="Tuple"/> containing 2 following elements.<br/>
    /// - A <see cref="List{T}"/> where <c>T</c> is <see cref="int"/>, representing the ids of
    /// the users those the created notification is distributed.<br/>
    /// - A <see cref="int"/> value, representing the id of the created notification.
    /// </returns>
    Task<(List<int>, int)> CreateAsync(NotificationType type, List<int> resourceIds);
}