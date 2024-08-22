namespace NATSInternal.Services;

public class NotificationService : INotificationService
{
    private readonly DatabaseContext _context;

    public NotificationService(
            DatabaseContext context)
    {
        _context = context;
    }

    public async Task<>
}