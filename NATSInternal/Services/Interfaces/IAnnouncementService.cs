namespace NATSInternal.Services.Interfaces;

/// <summary>
/// A service to handle announcements.
/// </summary>
public interface IAnnouncementService
{
    /// <summary>
    /// Retrieves a list of announcements, based on the specified filtering, sorting and
    /// paginating conditions.
    /// </summary>
    /// <param name="requestDto">
    /// An instance of the <see cref="AnnouncementListRequestDto"/> class, contaning the
    /// conditions for the results.
    /// </param>
    /// <returns>
    /// An instance of the <see cref="AnnouncementListResponseDto"/> class, containing the
    /// results and the additional information for pagination.
    /// </returns>
    Task<AnnouncementListResponseDto> GetListAsync(AnnouncementListRequestDto requestDto);

    /// <summary>
    /// Retrieves the details of a specific announcement, based on its id.
    /// </summary>
    /// <param name="id">
    /// An <see cref="int"/> representing the id of the announcement to retrieve.
    /// </param>
    /// <returns>
    /// An instance of the <see cref="AnnouncementResponseDto"/> class, containing the details
    /// of the announcement.
    /// </returns>
    /// <exception cref="ResourceNotFoundException">
    /// Throws when the announcement with the specified doesn't exist or has already been
    /// deleted.
    /// </exception>
    Task<AnnouncementResponseDto> GetDetailAsync(int id);

    /// <summary>
    /// Creates a new announcement based on the specified data.
    /// </summary>
    /// <param name="requestDto">
    /// An instance of the <see cref="AnnouncementUpsertRequestDto"/> class, containing the
    /// data for the creating operation.
    /// </param>
    /// <returns>
    /// An <see cref="int"/> represeting the id of the new announcement.
    /// </returns>
    /// <exception cref="ConcurrencyException">
    /// Throws when a concurrency-related conflict occurs during the operation.
    /// </exception>
    Task<int> CreateAsync(AnnouncementUpsertRequestDto requestDto);

    /// <summary>
    /// Updates an existing announcement based on its id and the specified data.
    /// </summary>
    /// <param name="id">
    /// An <see cref="int"/> representing the id of the announcement to update.
    /// </param>
    /// <param name="requestDto">
    /// An instance of the <see cref="AnnouncementUpsertRequestDto"/> class, containing the
    /// data for the updating operation.
    /// </param>
    /// <exception cref="ResourceNotFoundException">
    /// Throws when the announcement with the specified id doesn't exist or has already been
    /// deleted.
    /// </exception>
    /// <exception cref="ConcurrencyException">
    /// Throws when a concurrency-related conflict occurs during the operation.
    /// </exception>
    Task UpdateAsync(int id, AnnouncementUpsertRequestDto requestDto);

    /// <summary>
    /// Deletes an existing announcement based on its id.
    /// </summary>
    /// <param name="id">
    /// An <see cref="int"/> representing the id of the announcement to delete.
    /// </param>
    /// <exception cref="ResourceNotFoundException">
    /// Throws when the announcement with the specified id doesn't exist or has already been
    /// deleted.
    /// </exception>
    Task DeleteAsync(int id);
}
