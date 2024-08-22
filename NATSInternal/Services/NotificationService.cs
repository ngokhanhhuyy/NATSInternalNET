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
    public async Task<NotificationListResponseDto> GetListAsync(NotificationListRequestDto requestDto)
    {
        // Initialize query.
        int currentUserId = _authorizationService.GetUserId();
        IQueryable<Notification> query = _context.Notifications
            .Include(n => n.ReadUsers).ThenInclude(u => u.ReceivedNotifications)
            .Where(n => n.ReceivedUsers.Select(u => u.Id).Contains(currentUserId));
        
        // Initialize response dto.
        NotificationListResponseDto responseDto = new NotificationListResponseDto();
        int resultCount = await query.CountAsync();
        if (resultCount == 0)
        {
            responseDto.PageCount = 0;
            return responseDto;
        }
        responseDto.PageCount = (int)Math.Ceiling((double)resultCount / requestDto.ResultsPerPage);
        responseDto.Items = await query
            .Select(user => new NotificationResponseDto(user, currentUserId))
            .Skip(requestDto.ResultsPerPage * (requestDto.Page - 1))
            .Take(requestDto.ResultsPerPage)
            .AsSplitQuery()
            .ToListAsync();
        
        return responseDto;
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
    
    // TODO: Implement notification creating mechanism.
    // public async Task<int> CreateAsync(NotificationType type, List<int> resourceIds)
    // {
    //     // Fetch the list of all users' ids.
    //     List<string>;
    //     
    //     // Initialize the entity.
    //     Notification notification = new Notification
    //     {
    //         Type = type,
    //         DateTime = DateTime.UtcNow.ToApplicationTime(),
    //         ResourceIds = resourceIds
    //         
    //     };
    //     
    //     ;
    //     
    // }
    
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