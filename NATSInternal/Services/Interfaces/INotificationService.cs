namespace NATSInternal.Services.Interfaces;

/// <summary>
/// A service to handle notifications.
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Gets a paginated list of notifications based on the specified request parameters.
    /// </summary>
    /// <param name="requestDto">
    /// The request parameters for fetching the expense list.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the notification list response DTO.
    /// </returns>
    Task<NotificationListResponseDto> GetListAsync(NotificationListRequestDto requestDto);
    
    /// <summary>
    /// Set the notification with the specified id as read.
    /// </summary>
    /// <param name="id">The id of the notification.</param>
    /// <exception cref="ResourceNotFoundException">
    /// Thrown when the notification with the specified id which belongs to the current user
    /// cannot be found.
    /// </exception>
    Task MarkAsReadAsync(int id);
    
    /// <summary>
    /// Create a notification which all users can receive with the specified
    /// notification type and resource ids.
    /// </summary>
    /// <param name="type">The type of the notification.</param>
    /// <param name="resourceIds">The id(s) of the interacted resource.</param>
    /// <returns>The id of the created notification.</returns>
    Task<int> CreateAsync(NotificationType type, List<int> resourceIds);
}