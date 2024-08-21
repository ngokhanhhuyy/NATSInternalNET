using System.Linq;

namespace NATSInternal.Services;

/// <inheritdoc />
public class AnnouncementService : IAnnouncementService
{
    private readonly DatabaseContext _context;
    private readonly IAuthorizationService _authorizationService;

    public AnnouncementService(
            DatabaseContext context,
            IAuthorizationService authorizationService)
    {
        _context = context;
        _authorizationService = authorizationService;
    }

    /// <inheritdoc />
    public async Task<AnnouncementListResponseDto> GetListAsync(
            AnnouncementListRequestDto requestDto)
    {
        // Initialize query statement.
        IQueryable<Announcement> query = _context.Announcements
            .Include(a => a.CreatedUser)
            .Include(a => a.ReadUsers);

        // Filter by new announcements only if specified.
        if (requestDto.NewAnnouncementsOnly)
        {
            int thisUserId = _authorizationService.GetUserId();
            query = query.Where(
                a => !a.ReadUsers.Select(ru => ru.Id).Contains(thisUserId));
        }

        // Order the results.
        query = query.OrderByDescending(a => a.StartingDateTime)
            .ThenByDescending(a => a.EndingDateTime);

        // Initialize response dto.
        AnnouncementListResponseDto responseDto = new AnnouncementListResponseDto();

        // Fetch the result count.
        int resultCount = await query.CountAsync();
        if (resultCount == 0)
        {
            responseDto.PageCount = 0;
            return responseDto;
        }

        responseDto.PageCount = (int)Math.Ceiling((double)resultCount / requestDto.ResultsPerPage);
        responseDto.Items = await query
            .Select(a => new AnnouncementResponseDto(a))
            .Skip(requestDto.ResultsPerPage * (requestDto.Page - 1))
            .Take(requestDto.ResultsPerPage)
            .AsSplitQuery()
            .ToListAsync();

        return responseDto;
    }
}