namespace NATSInternal.Services.Interfaces;

/// <summary>
/// A service to handle the role-related operations.
/// </summary>
public interface IRoleService
{
    /// <summary>
    /// Get a list of all roles' basic information.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation, which result is an
    /// instance of the <see cref="RoleListResponseDto"/> class, containing the results.
    /// </returns>
    Task<RoleListResponseDto> GetListAsync();
}
