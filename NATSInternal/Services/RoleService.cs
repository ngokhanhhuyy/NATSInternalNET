namespace NATSInternal.Services;

public class RoleService : IRoleService
{
    private readonly DatabaseContext _context;

    public RoleService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<RoleListResponseDto> GetListAsync()
    {
        RoleListResponseDto responseDto = new RoleListResponseDto
        {
            Items = await _context.Roles
                .Select(r => new RoleBasicResponseDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    DisplayName = r.DisplayName,
                    PowerLevel = r.PowerLevel
                }).ToListAsync()
        };

        return responseDto;
    }
}
