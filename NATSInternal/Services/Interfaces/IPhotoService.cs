namespace NATSInternal.Services.Interfaces;

public interface IPhotoService
{
    /// <summary>
    /// Create a photo and save on the photo folder with subfolder {ntityName}.
    /// The name of the photo will be the time when this method is called.
    /// </summary>
    /// <param name="content">
    /// An array of byte representing the photo file after reading file from the request.
    /// </param>
    /// <param name="folderName">
    /// The name of the folder inside /images/front-pages directory that this photo will be saved.
    /// </param>
    /// <param name="cropToSquare">
    /// Determine if the image should be cropped into square image.
    /// </param>
    /// <returns>The relative path (URL) to the created photo on the server</returns>
    /// <example>~/photos/users/{id}.jpg</example>
    Task<string> CreateAsync(byte[] content, string folderName, bool cropToSquare);

    /// <summary>
    /// Create a photo and save on the photo folder with subfolder {ntityName}.
    /// The name of the photo will be the time when this method is called.
    /// </summary>
    /// <param name="content">
    /// An array of byte representing the photo file after reading file from the request.
    /// </param>
    /// <param name="folderName">
    /// The name of the folder inside /images/front-pages directory that this photo will be saved.
    /// </param>
    /// <param name="options">
    /// The object contains the desired with, height and aspect ratio of the image after being processed.
    /// </param>
    /// <returns>The relative path (URL) to the created photo on the server</returns>
    /// <example>~/photos/users/{id}.jpg</example>
    Task<string> CreateAsync(byte[] content, string folderName, double aspectRatio);

    /// <summary>
    /// Delete an existing photo by relative path on the server. The relative path is usually stored in
    /// the database and associated with some of the main entities.
    /// </summary>
    /// <param name="relativePath">
    /// The full path to the photo on the server, usually in wwwroot/photos/{entityName}/
    /// </param>
    void Delete(string relativePath);
}