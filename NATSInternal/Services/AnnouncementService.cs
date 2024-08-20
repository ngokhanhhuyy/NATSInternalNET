namespace NATSInternal.Services;

public class AnnouncementService
{
    private readonly DatabaseContext _context;

    public AnnouncementService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<AnnouncementResponseDto> GetListAsync(
            AnnouncementListRequestDto requestDto)
    {
        // Initialize query statement.
        IQueryable<Announcement> query = _context.Announcements;

        // Filter by new announcements only if specified.
        if (requestDto.NewAnnouncementsOnly)
        {
            query = query.Where()
        }
    }
}