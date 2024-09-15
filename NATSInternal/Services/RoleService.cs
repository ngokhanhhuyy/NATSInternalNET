namespace NATSInternal.Services;

/// <inheritdoc />
public class RoleService : IRoleService
{
    private readonly DatabaseContext _context;

    public RoleService(DatabaseContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<RoleListResponseDto> GetListAsync()
    {
        RoleListResponseDto responseDto = new RoleListResponseDto
        {
            Items = await _context.Roles
                .Select(r => new RoleBasicResponseDto(r))
                .ToListAsync()
        };

        return responseDto;
    }
}