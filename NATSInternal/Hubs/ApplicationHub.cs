using System.Globalization;
using System.Security.Claims;

namespace NATSInternal.Hubs;

/// <summary>
/// A central <c>Hub</c> for the real-time connection between the clients and the
/// application to communicate about notifications and resource accessing events.
/// </summary>
[Authorize]
public class ApplicationHub : Hub
{
    /// <summary>
    /// An instance of a <c><see cref="IUserService"/></c> interface's implementation which is
    /// injected using dependency injection.
    /// </summary>
    private readonly IUserService _userService;

    /// <summary>
    /// An instance of a <c><see cref="INotificationService"/></c> interface's implementation
    /// which is injected using dependency injection.
    /// </summary>
    private readonly INotificationService _notificationService;

    /// <summary>
    /// A <c>Dictionary</c> contaning resources' information and the connection ids of
    /// the users connecting to each resource.
    /// </summary>
    /// <remarks>
    /// The keys are the instances of <c><see cref="Resource"/></c>.<br/>
    /// The values are the <c><see cref="HashSet{string}"/></c> contanining all the connection
    /// ids of the users connecting to the each resource.
    /// </remarks>
    private static readonly Dictionary<Resource, HashSet<string>> _resourceConnections;

    /// <summary>
    /// A <c>Dictionary</c> containing all the ids of the users connecting to the hub
    /// and the connection ids of each user.
    /// </summary>
    /// <remarks>
    /// The keys are the <c><see cref="int"/></c> values representing the id of each user.<br/>
    /// The values is the <c><see cref="HashSet{T}"/></c> where <c>TKey</c> is
    /// <c><see cref="string"/></c> contanining all the connection ids associated to the user.
    /// </remarks>
    private static readonly Dictionary<int, HashSet<string>> _userConnections;

    /// <summary>
    /// Initializes a new instance of the <c><see cref="ApplicationHub"/></c> class
    /// for each connection handling.
    /// </summary>
    /// <param name="userService">
    /// An instance of the <c><see cref="IUserService"/></c>
    /// </param>
    public ApplicationHub(
        IUserService userService,
        INotificationService notificationService) : base()
    {
        _userService = userService;
        _notificationService = notificationService;
    }

    /// <summary>
    /// Initializes the static members of the <c><see cref="ApplicationHub"/></c>
    /// </summary>
    static ApplicationHub()
    {
        _userConnections = new Dictionary<int, HashSet<string>>();
        _resourceConnections = new Dictionary<Resource, HashSet<string>>();
    }

    /// <summary>
    /// The id of the user who is associated to this hub instance.
    /// </summary>
    private int UserId => int.Parse(Context.UserIdentifier);

    /// <summary>
    /// Get all the ids of all the users who are connecting to the hub.
    /// </summary>
    public static List<int> ConnectingUserIds => _userConnections.Keys.ToList();

    /// <summary>
    /// Handle when a user connects to the hub.
    /// </summary>
    /// <returns>
    /// A <c><see cref="Task"/></c> operation representing the async operation.
    /// </returns>
    /// <remarks>
    /// Add the current user id as key and a list containing the current connection id as
    /// value to the user connection dictionary.
    /// </remarks>
    public override async Task OnConnectedAsync()
    {
        // Check if the user is already been connecting to the hub with different
        // connection ids.
        _userConnections.TryGetValue(UserId, out HashSet<string> connectionIds);
        if (connectionIds == null)
        {
            connectionIds = new HashSet<string>
            {
                Context.ConnectionId
            };

            // Add the current connection id to the user's connection id list.
            _userConnections.Add(UserId, connectionIds);
        }

        await LogConnectionStatus(true);
    }

    /// <summary>
    /// Handle when a user disconnects from the hub.
    /// </summary>
    /// <param name="exception">
    /// The <c>Exception</c> storing the reason of the disconnection.
    /// </param>
    /// <returns>
    /// A <c><see cref="Task"/></c> operation representing the async operation.
    /// </returns>
    /// <remarks>
    /// Removes the current user id from the user ids dictionary and removes and all of the
    /// user's connection ids from the resource access connection id list.
    /// </remarks>
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        // Prepare a list of connections which are to be notified.
        List<string> connectionIdsToBeNotified = new List<string>();

        // Remove the current connection id from the resource access dictionary if exists.
        List<KeyValuePair<Resource, HashSet<string>>> pairs;
        pairs = _resourceConnections.ToList();
        foreach (KeyValuePair<Resource, HashSet<string>> pair in pairs)
        {
            bool isRemoved = pair.Value.Remove(Context.ConnectionId);
            if (isRemoved)
            {
                await Clients
                    .Clients(pair.Value)
                    .SendAsync("ResourceAccessFinished", pair.Key);
            }

            if (!pair.Value.Any())
            {
                _resourceConnections.Remove(pair.Key);
            }
        }

        // Remove the user id from the connection id dictionary if the list containing
        // the connection ids contans only 1 element.
        _userConnections.ToList().ForEach(pair =>
        {
            pair.Value.Remove(Context.ConnectionId);
            if (!pair.Value.Any())
            {
                _userConnections.Remove(pair.Key);
            }
        });

        await LogConnectionStatus(false);
    }

    /// <summary>
    /// Notifies other users who are accessing the specified resource that this user has
    /// started his access to the same resource and store the connection id to the storage.
    /// </summary>
    /// <param name="resource">
    /// A <c><see cref="Resource"></c> object containing name, primary id and secondary
    /// as the identity of the accessing resource.
    /// </param>
    /// <returns>A <c><see cref="Task"/></c> object representing the asynchronous opeartion.</returns>
    public async Task StartResourceAccess(Resource resource)
    {
        // Get the list of all connection ids connecting to the specified resource.
        bool hasResourceBeenAdded = _resourceConnections
            .TryGetValue(resource, out HashSet<string> connectionIds);
        if (!hasResourceBeenAdded)
        {
            connectionIds = new HashSet<string>();
            _resourceConnections.Add(resource, connectionIds);
        }

        // Add the current connection id to the resource connection id list.
        if (!connectionIds.Contains(Context.ConnectionId))
        {
            connectionIds.Add(Context.ConnectionId);
        }

        // Get a list of all connecting users' information.
        List<int> connectingUserIds = _userConnections
            .Where(pair => pair.Value.Any(id => connectionIds.Contains(id)))
            .Select(pair => pair.Key)
            .ToList();
        UserListResponseDto listResponseDto = await _userService
            .GetListAsync(connectingUserIds);

        // Notify other accessing users that this user's access session has started.
        await Clients
            .Users(connectingUserIds
                .Where(id => id != UserId)
                .Select(id => id.ToString()).ToList())
            .SendAsync(
                "Other.ResourceAccessStarted",
                resource,
                listResponseDto.Results.Single(u => u.Id == UserId));

        // Send the list of all connecting users back to the caller.
        await Clients
            .Client(Context.ConnectionId)
            .SendAsync("Self.ResourceAccessStarted", resource, listResponseDto);
    }

    /// <summary>
    /// Notifies other users who are accessing the specified resource that this user has
    /// finished his access to the same resource and remove the connection id from the
    /// connection id storage.
    /// </summary>
    /// <param name="resource">
    /// A <c><see cref="Resource"></c> object containing name, primary id and secondary
    /// as the identity of the accessing resource.
    /// </param>
    /// <returns>A <c><see cref="Task"/></c> object representing the asynchronous opeartion.</returns>
    public async Task FinishResourceAccess(Resource resource)
    {
        // Get the list of connection ids of the specified resource.
        _resourceConnections.TryGetValue(resource, out HashSet<string> connectionIds);

        if (connectionIds != null)
        {
            // Remove the current connection id from the list.
            connectionIds.Remove(Context.ConnectionId);

            // Check if the current user is still accessing the resource through any other
            // connection ids.
            bool isUserStillConnecting = connectionIds
                .Any(id => FindUserIdFromConnectionId(id) == UserId);
            if (!isUserStillConnecting)
            {
                // Notify others users accessing the resource that the user has finished his
                // access.
                List<string> otherUserIds = _userConnections
                    .Where(pair => pair.Value.Any(id => connectionIds.Contains(id)))
                    .Select(pair => pair.Key.ToString())
                    .ToList();
                await Clients.Users(otherUserIds).SendAsync(
                    "Other.ResourceAccessFinished",
                    resource,
                    UserId);
            }

            // Check if the list is empty after the removal. If it is, remove the entire
            // resource from the resource access dictionary.
            if (!connectionIds.Any())
            {
                _resourceConnections.Remove(resource);
            }
        }
    }

    /// <summary>
    /// Get a list of user ids of the users who are accessing the specified resource.
    /// </summary>
    /// <param name="resource">The resource which the retrieving users are accessing.</param>
    /// <returns>A list of <c><see cref="int"/></c> representing the id of the users.</returns>
    public static List<int> GetUserIdsConnectingToResource(Resource resource)
    {
        // Find the connection ids of the connections to the resource.
        _resourceConnections.TryGetValue(resource, out HashSet<string> connectionIds);

        // Find the user ids by the retrieved connection ids.
        List<int> userIdsConnectingToResource = new List<int>();
        if (connectionIds != null)
        {
            userIdsConnectingToResource.AddRange(_userConnections
                .Where(pair => pair.Value.Any(id => connectionIds.Contains(id)))
                .Select(pair => pair.Key)
                .ToList());
        }

        return userIdsConnectingToResource;
    }

    /// <summary>
    /// Log the status of of the user's connection who has just connected to or disconnected
    /// from the hub.
    /// </summary>
    /// <param name="isConnected">
    /// <c>true</c> to indicate that the user has connected, otherwise, <c>false</c>.
    /// </param>
    /// <returns>A Task object representing the asynchronous operation.</returns>
    private async Task LogConnectionStatus(bool isConnected)
    {
        string userId = Context.User!.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        string userName = Context.User!.FindFirst(ClaimTypes.Name)!.Value;
        Console.BackgroundColor = isConnected ? ConsoleColor.Green : ConsoleColor.Red;
        Console.ForegroundColor = ConsoleColor.White;
        await Console.Out.WriteAsync("SignalR");
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.White;
        await Console.Out.WriteAsync(" ");
        Console.BackgroundColor = ConsoleColor.White;
        Console.ForegroundColor = ConsoleColor.Black;
        await Console.Out.WriteAsync(DateTime.UtcNow.ToApplicationTime()
            .ToString(CultureInfo.InvariantCulture));
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.White;
        string connectionStatus = isConnected ? "Connected" : "Disconnected";
        await Console.Out.WriteLineAsync(
            $" {connectionStatus} NotificationHub ({userName}#{userId})");
    }

    /// <summary>
    /// Find the id of the user who owns the specified connection id.
    /// </summary>
    /// <param name="connectionId">
    /// The connection id associated to the finding user.
    /// </param>
    /// <returns>An <c><see cref="int"/></c> reprensenting the id of the user.</returns>
    private int FindUserIdFromConnectionId(string connectionId)
    {
        return _userConnections
            .Where(pair => pair.Value.Contains(connectionId))
            .Select(pair => pair.Key)
            .FirstOrDefault();
    }
}