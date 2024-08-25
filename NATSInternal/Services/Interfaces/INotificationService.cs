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
    /// Get a single notification with the specified id. The current user who has sent the request
    /// must be in the list of the notification's received users.
    /// </summary>
    /// <param name="id">The id of the notification.</param>
    /// <exception cref="ResourceNotFoundException">
    /// Thrown when the notification with the specified id which belongs to the current user
    /// cannot be found.
    /// </exception>
    Task<NotificationResponseDto> GetSingleAsync(int id);
    
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
    /// <returns>
    /// A tuple containing 2 elements. The first one is a list of the users
    /// those the created notification is distributed. The second one is the id
    /// of the created notification.
    /// </returns>
    Task<(List<int>, int)> CreateAsync(NotificationType type, List<int> resourceIds);
}