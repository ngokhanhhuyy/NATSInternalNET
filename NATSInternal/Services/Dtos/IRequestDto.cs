namespace NATSInternal.Services.Dtos;

/// <summary>
/// A DTO class containing the data mapped from the requests for data-related operations.
/// </summary>
/// <typeparam name="TRequestDto">
/// Representing the type of the DTO class which inherits from this interface.
/// </typeparam>
public interface IRequestDto<out TRequestDto>
{
    /// <summary>
    /// Transform the values of all properties which represent the absence of data or meanless
    /// value into the default value.
    /// </summary>
    /// <returns>
    /// The instance of the class which inherits from the <see cref="IRequestDto"/> interface
    /// and on which the method is called.
    /// </returns>
    TRequestDto TransformValues();
}