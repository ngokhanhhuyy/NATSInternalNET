namespace NATSInternal.Hubs.ResourceAccess;

public class ResourceAccessStorage
{

    /// <summary>
    /// A <c>Dictionary</c> contaning resources' information and the connection ids of
    /// the users connecting to each resource.
    /// </summary>
    /// <remarks>
    /// The key is an <see cref="Resource"/> contaning information of the
    /// resource.<br/>
    /// The value is a list contanining all the connection ids of the users connecting
    /// to the resource.
    /// </remarks>
    private static readonly Dictionary<Resource, List<string>> _resourceConnectionDictionary;
}