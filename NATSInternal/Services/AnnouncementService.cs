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
            .OrderByDescending(a => a.StartingDateTime)
                .ThenByDescending(a => a.EndingDateTime)
                .ThenByDescending(a => a.CreatedDateTime);

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

    /// <inheritdoc />
    public async Task<AnnouncementResponseDto> GetDetailAsync(int id)
    {
        return await _context.Announcements
            .Include(a => a.CreatedUser)
            .Where(a => a.Id == id)
            .Select(a => new AnnouncementResponseDto(a))
            .SingleOrDefaultAsync()
            ?? throw new ResourceNotFoundException();
    }

    /// <inheritdoc />
    public async Task<int> CreateAsync(AnnouncementUpsertRequestDto requestDto)
    {
        // Initialize the entity.
        DateTime startingDateTime = requestDto.StartingDateTime
            ?? DateTime.UtcNow.ToApplicationTime();
        Announcement announcement = new Announcement
        {
            Category = requestDto.Category,
            Title = requestDto.Title,
            Content = requestDto.Content,
            StartingDateTime = startingDateTime,
            EndingDateTime = startingDateTime
                .AddMinutes(requestDto.IntervalInMinutes),
            CreatedUserId = _authorizationService.GetUserId()
        };
        _context.Announcements.Add(announcement);

        // Perform the creating opeartion.
        try
        {
            await _context.SaveChangesAsync();
            return announcement.Id;
        }
        catch (DbUpdateException exception)
        when (exception.InnerException is MySqlException sqlException)
        {
            HandleCreateOrUpdateException(sqlException);

            throw;
        }
    }

    /// <inheritdoc />
    public async Task UpdateAsync(int id, AnnouncementUpsertRequestDto requestDto)
    {
        // Fetch the entity from the database and ensure it exists.
        Announcement announcement = await _context.Announcements
            .SingleOrDefaultAsync(a => a.Id == id)
            ?? throw new ResourceNotFoundException();

        // Updating the entity's properties.
        announcement.Category = requestDto.Category;
        announcement.Title = requestDto.Title;
        announcement.Content = requestDto.Content;
        
        if (requestDto.StartingDateTime.HasValue)
        {
            announcement.StartingDateTime = requestDto.StartingDateTime
                ?? DateTime.UtcNow.ToApplicationTime();
            announcement.EndingDateTime = announcement.StartingDateTime
                .AddMinutes(requestDto.IntervalInMinutes); 
        }
        
        // Save changes.
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException exception)
        when (exception.InnerException is MySqlException sqlException)
        {
            HandleCreateOrUpdateException(sqlException);

            throw;
        }
    }
    
    /// <inheritdoc />
    public async Task DeleteAsync(int id)
    {
        int affectedRows = await _context.Announcements
            .Where(a => a.Id == id)
            .ExecuteDeleteAsync();

        if (affectedRows == 0)
        {
            throw new ResourceNotFoundException();
        }
    }

    /// <summary>
    /// Handle the exception thrown from the database during the creating
    /// or updating operation and convert it into the appropriate exception.
    /// </summary>
    /// <param name="exception">
    /// The exception thrown by the database.
    /// </param>
    /// <exception cref="ConcurrencyException">
    /// Thrown when there is some concurrent conflict during the operation.
    /// </exception>
    private void HandleCreateOrUpdateException(MySqlException exception)
    {
        SqlExceptionHandler exceptionHandler = new SqlExceptionHandler();
        exceptionHandler.Handle(exception);
        if (exceptionHandler.IsForeignKeyNotFound)
        {
            throw new ConcurrencyException();
        }
    }
}