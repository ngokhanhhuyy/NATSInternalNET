namespace NATSInternal.Hubs.ResourceAccess;

/// <summary>
/// A <c>Dictionary</c> storing user ids and all the associated connection ids.
/// </summary>
/// <remarks>
/// The dictionary key is the user id, represented by an <c><see cref="int"/></c>.
/// The dictionary value is a list of connection ids associated with that user, represented
/// by a <see cref="List{string}"/>.
/// </remarks>
public class UserConnectionDictionary : Dictionary<int, List<string>>
{
    /// <summary>
    /// The id of the user who is the owner of the <c>UserConnectionIds</c>.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// A list containing the connections id associated to the <c>UserId</c>.
    /// </summary>
    public List<string> UserConnectionIds { get; set; }
}