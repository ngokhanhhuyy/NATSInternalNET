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
}
