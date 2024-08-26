namespace NATSInternal.Services;

/// <inheritdoc cref="INotificationService" />
public class NotificationService : INotificationService
{
    private readonly DatabaseContext _context;
    private readonly IAuthorizationService _authorizationService;

    public NotificationService(
            DatabaseContext context,
            IAuthorizationService authorizationService)
    {
        _context = context;
        _authorizationService = authorizationService;
    }
    
    /// <inheritdoc />
    public async Task<NotificationListResponseDto> GetListAsync(
            NotificationListRequestDto requestDto)
    {
        // Initialize query.
        int currentUserId = _authorizationService.GetUserId();
        IQueryable<Notification> query = _context.Notifications
            .Include(n => n.CreatedUser)
            .Include(n => n.ReadUsers).ThenInclude(u => u.ReceivedNotifications)
            .OrderByDescending(n => n.DateTime)
            .Where(n => n.ReceivedUsers.Select(u => u.Id).Contains(currentUserId));

        // Filter by unread notifications only.
        if (requestDto.UnreadOnly)
        {
            query = query
                .Where(n => !n.ReadUsers
                    .Select(u => u.Id)
                    .Contains(currentUserId));
        }
        
        // Initialize response dto.
        NotificationListResponseDto responseDto = new NotificationListResponseDto();
        int resultCount = await query.CountAsync();
        if (resultCount == 0)
        {
            responseDto.PageCount = 0;
            return responseDto;
        }
        responseDto.PageCount = (int)Math.Ceiling(
            (double)resultCount / requestDto.ResultsPerPage);
        responseDto.Items = await query
            .Select(user => new NotificationResponseDto(user, currentUserId))
            .Skip(requestDto.ResultsPerPage * (requestDto.Page - 1))
            .Take(requestDto.ResultsPerPage)
            .AsSplitQuery()
            .ToListAsync();
        
        return responseDto;
    }
    
    public async Task<NotificationResponseDto> GetSingleAsync(int id)
    {
        int currentUserId = _authorizationService.GetUserId();
        return await _context.Notifications
            .Include(n => n.CreatedUser)
            .Include(n => n.ReadUsers).ThenInclude(u => u.ReceivedNotifications)
            .OrderBy(n => n.Id)
            .Where(n => n.ReceivedUsers.Select(u => u.Id).Contains(currentUserId))
            .Where(n => n.Id == id)
            .Select(n => new NotificationResponseDto(n, currentUserId))
            .SingleOrDefaultAsync()
            ?? throw new ResourceNotFoundException();
    }
    
    /// <inheritdoc />
    public async Task MarkAsReadAsync(int id)
    {
        // Fetch the current user entity.
        User currentUser = await GetCurrentUser();
        
        // Fetch the notification entity.
        Notification notification = await _context.Notifications
            .Include(n => n.ReceivedUsers)
            .Include(n => n.ReadUsers)
            .Where(n => n.Id == id)
            .Where(n => n.ReceivedUsers.Select(u => u.Id).Contains(currentUser.Id))
            .SingleOrDefaultAsync()
            ?? throw new ResourceNotFoundException();
        
        // Add the current user to the notification's read user list.
        if (!notification.ReadUsers.Select(u => u.Id).Contains(currentUser.Id))
        {
            notification.ReadUsers.Add(currentUser);
            
            // Save the changes.
            await _context.SaveChangesAsync();
        }
    }
    
    /// <inheritdoc />
    public async Task MarkAllAsReadAsync()
    {
        // Fetch the current user entity.
        User currentUser = await GetCurrentUser();
        
        // Fetch the notification entity.
        List<Notification> notifications = await _context.Notifications
            .Include(n => n.ReceivedUsers)
            .Include(n => n.ReadUsers)
            .Where(n => n.ReceivedUsers.Select(u => u.Id).Contains(currentUser.Id))
            .Where(n => !n.ReadUsers.Select(u => u.Id).Contains(currentUser.Id))
            .ToListAsync();
        
        // Add the current user to each notification's read user list.
        foreach (Notification notification in notifications)
        {
            if (!notification.ReadUsers.Select(u => u.Id).Contains(currentUser.Id))
            {
                notification.ReadUsers.Add(currentUser);
                
                // Save the changes.
                await _context.SaveChangesAsync();
            }
        }
    }
    /// <inheritdoc />
    public async Task<(List<int>, int)> CreateAsync(
            NotificationType type,
            List<int> resourceIds)
    {
        // Fetch the list of all users' ids.
        List<int> userIds = await _context.Users
            .Where(u => !u.IsDeleted)
            .Select(u => u.Id)
            .ToListAsync();
        
        NotificationType[] selfCreatedNotificationType =
        {
            NotificationType.UserBirthday,
            NotificationType.UserJoiningDateAnniversary,
            NotificationType.CustomerBirthday
        };
        
        // Use transaction for atomic operations.
        await using IDbContextTransaction transaction = await _context.Database
            .BeginTransactionAsync();
        
        // Determine the interacted user id value.
        int? createdUserId = null;
        if (!selfCreatedNotificationType.Contains(type))
        {
            createdUserId = _authorizationService.GetUserId();
        }

        // Initialize the entity.
        Notification notification = new Notification
        {
            Type = type,
            DateTime = DateTime.UtcNow.ToApplicationTime(),
            ResourceIds = resourceIds,
            CreatedUserId = createdUserId
        };
        _context.Notifications.Add(notification);

        // Initialize the relationship between all users (as notification receivers)
        // and the notification.
        foreach (int userId in userIds)
        {
            NotificationReceivedUser notificationReceivedUser;
            notificationReceivedUser = new NotificationReceivedUser
            {
                ReceivedNotification = notification,
                ReceivedUserId = userId
            };
            _context.NotificationReceivedUsers.Add(notificationReceivedUser);
        }
        // Save the notification received user entity.
        await _context.SaveChangesAsync();

        // Commit the transaction and finish the operation.
        await transaction.CommitAsync();
        return (userIds, notification.Id);
    }
    
    /// <summary>
    /// Fetch the entity of the current user.
    /// </summary>
    /// <returns>The entity of the current user.</returns>
    private async Task<User> GetCurrentUser()
    {
        int currentUserId = _authorizationService.GetUserId();
        return await _context.Users
            .SingleOrDefaultAsync(u => u.Id == currentUserId);
    }
}