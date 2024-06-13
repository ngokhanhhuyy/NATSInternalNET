namespace NATSInternal.Services.Interfaces;

public interface IRoleService {
    Task<RoleListResponseDto> GetListAsync();
}
