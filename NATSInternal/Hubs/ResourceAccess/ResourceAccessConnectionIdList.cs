namespace NATSInternal.Hubs;

/// <summary>
/// A data class to store a resource's ids and all connection id
/// </summary>
public class ResourceAccessConnectionIdList
{
    /// <summary>
    /// Representing the primary id of the resource.
    /// </summary>
    public int ResourcePrimaryId { get; init; }

    /// <summary>
    /// Representing the 
    /// </summary>
    public int? ResourceSecondaryId { get; init; } = null;
    public List<string> ConnectionIds { get; init; } = new List<string>();
}