namespace NATSInternal.Services.Interfaces;

/// <summary>
/// A service to handle announcements.
/// </summary>
public interface IAnnouncementService
{
    /// <summary>
    /// Get a list of announcements which each announcement contains basic
    /// information with filtering condition.
    /// </summary>
    /// <param name="requestDto">
    /// An object containing filtering condition for the results.
    /// </param>
    /// <returns>The list of announcements.</returns>
    Task<AnnouncementListResponseDto> GetListAsync(AnnouncementListRequestDto requestDto);

    /// <summary>
    /// Get the fully detailed information of the announcement with the
    /// specified id.
    /// </summary>
    /// <param name="id">The id of the announcement to be retrieve.</param>
    /// <returns>An object containing the information of the announcement.</returns>
    /// <exception cref="ResourceNotFoundException">
    /// Thrown when the announcement cannot be found.
    /// </exception>
    Task<AnnouncementResponseDto> GetDetailAsync(int id);

    /// <summary>
    /// Create a new announcement using the data provided from
    /// the request.
    /// </summary>
    /// <param name="requestDto">
    /// An object containing the data for a new announcement, provided
    /// from the request.
    /// </param>
    /// <returns>
    /// The id of the created announcement.
    /// </returns>
    /// <exception cref="ConcurrencyException">
    /// Thrown when there is some concurrent conflict during the operation.
    /// </exception>
    Task<int> CreateAsync(AnnouncementUpsertRequestDto requestDto);

    /// <summary>
    /// Update the announcement which has the specified id with the data
    /// provided from the request.
    /// </summary>
    /// <param name="id">The id of the announcement to be updated.</param>
    /// <param name="requestDto">
    /// An object containing the data to be updated, provided from the request.
    /// </param>
    /// <exception cref="ResourceNotFoundException">
    /// Thrown when the announcement with the specified id cannot be found.
    /// </exception>
    /// <exception cref="ConcurrencyException">
    /// Thrown when there is some concurrent conflict during the operation.
    /// </exception>
    Task UpdateAsync(int id, AnnouncementUpsertRequestDto requestDto);

    /// <summary>
    /// Delete the announcement which has the specified id.
    /// </summary>
    /// <param name="id">The id of the announcement to be updated.</param>
    /// <exception cref="ResourceNotFoundException">
    /// Thrown when the announcement with the specified id cannot be found.
    /// </exception>
    Task DeleteAsync(int id);
}
