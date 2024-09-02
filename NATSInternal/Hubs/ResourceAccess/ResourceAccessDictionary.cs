namespace NATSInternal.Hubs.ResourceAccess;

/// <summary>
/// A <c>Dictionary</c> containing resources and associated connection ids.
/// </summary>
public class ResourceAccessDictionary : Dictionary<Resource, List<string>> { }