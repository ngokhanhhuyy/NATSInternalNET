namespace NATSInternal.Hubs.ResourceAccess;

/// <summary>
/// A data class containing the name and the id(s) of the resource.
/// </summary>
public class Resource
{
    /// <summary>
    /// The type of the resource (User, Customer, Product, Order, ...).
    /// </summary>
    public string Type { get; init; }

    /// <summary>
    /// The primary id of the resource.
    /// </summary>
    public int PrimaryId { get; init; }

    /// <summary>
    /// The secondary id of the resource.
    /// </summary>
    /// <remarks>
    /// Only <c>DebtIncrrence</c> or <c>DebtPayment</c> has secondary id.
    /// </remarks>
    public int? SecondaryId { get; init; }

    /// <summary>
    /// Indicates the mode of the access (detail or update).
    /// </summary>
    public ResourceAccessMode Mode { get; init; }

    /// <summary>
    /// Determines of the specified object has the same type and is equal to this
    /// resource.
    /// </summary>
    /// <param name="obj">The object to be compared with the current resource.</param>
    /// <returns>
    /// <c>true</c> if the specified object is equal to the current resource. Otherwise,
    /// <c>false</c>.
    /// </returns>
    public override bool Equals(object obj)
    {
        if (obj is Resource otherResource)
        {
            return Type == otherResource.Type
                && PrimaryId == otherResource.PrimaryId
                && SecondaryId == otherResource.SecondaryId
                && Mode == otherResource.Mode;
        }

        return false;
    }

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>
    /// A hash code for the current resource, which is based on the <see cref="Type"/>,
    /// <see cref="PrimaryId"/>, and <see cref="SecondaryId"/> properties.
    /// </returns>
    /// <remarks>
    /// This implementation of <see cref="GetHashCode"/> uses the
    /// <see cref="HashCode.Combine"/> method to generate a hash code that uniquely
    /// identifies a <see cref="Resource"/> object based on its <see cref="Type"/>,
    /// <see cref="PrimaryId"/>, and <see cref="SecondaryId"/>.
    /// </remarks>
    public override int GetHashCode()
    {
        return HashCode.Combine(Type, PrimaryId, SecondaryId, Mode);
    }
}