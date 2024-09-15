namespace NATSInternal.Services.Interfaces;

/// <summary>
/// A service to handle the photo-related operations.
/// </summary>
public interface IPhotoService
{
    /// <summary>
    /// Creates a new photo and save it into a specific folder.
    /// </summary>
    /// <remarks>
    /// The folder which name is specified by the value of the <c>folderName</c> argument is
    /// placed under the directory <c>/wwwroot/photos</c>. The name of the photo file is a
    /// string which contains the combination of the created datetime and a UUID.
    /// </remarks>
    /// <returns>
    /// <param name="content">
    /// An <see cref="Array"/> of <see cref="byte"/>, contanining the data of the photo after
    /// reading the file provided in the request.
    /// </param>
    /// <param name="folderName">
    /// A <see cref="string"/> representing the name of the folder to save the photo.
    /// </param>
    /// <param name="cropToSquare">
    /// A <see cref="bool"/> value indicating if the image should be cropped into square image.
    /// </param>
    /// A <see cref="Task"/> representing the asynchronous operation, which result is a string
    /// representing the relative path (URL) to the created photo in the specified folder.
    /// </returns>
    Task<string> CreateAsync(byte[] content, string folderName, bool cropToSquare);

    /// <summary>
    /// Creates a new photo and save it into a specific folder.
    /// </summary>
    /// <remarks>
    /// The folder which name is specified by the value of the <c>folderName</c> argument is
    /// placed under the directory <c>/wwwroot/photos</c>. The name of the photo file is a
    /// string which contains the combination of the created datetime and a UUID.
    /// </remarks>
    /// <param name="content">
    /// An <see cref="Array"/> of <see cref="byte"/>, contanining the data of the photo after
    /// reading the file provided in the request.
    /// </param>
    /// <param name="folderName">
    /// A <see cref="string"/> representing the name of the folder to save the photo.
    /// </param>
    /// <param name="aspectRatio">
    /// A <see cref="double"/> value indicate the aspect ratio that the operation should base
    /// on to crop the photo.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation, which result is a string
    /// representing the relative path (URL) to the created photo in the specified folder.
    /// </returns>
    Task<string> CreateAsync(byte[] content, string folderName, double aspectRatio);

    /// <summary>
    /// Deletes an existing photo by the relative path on the server.
    /// </summary>
    /// <param name="relativePath">
    /// A <see cref="string"/> representing the full path to the photo on the server, usually
    /// in <c>wwwroot/photos/{entityName}/</c>
    /// </param>
    void Delete(string relativePath);
}