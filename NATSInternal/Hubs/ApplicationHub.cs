using System.Globalization;
using System.Security.Claims;
using NATSInternal.Hubs.ResourceAccess;

namespace NATSInternal.Hubs;

/// <summary>
/// A central <c>Hub</c> for the real-time connection between the clients and the
/// application to communicate about notifications and resource accessing events.
/// </summary>
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ApplicationHub : Hub
{
    /// <summary>
    /// An instance of the <c>IUserClass</c> which is injected to this class instance
    /// by dependency injection.
    /// </summary>
    private readonly IUserService _userService;

    /// <summary>
    /// The id of the user who is associated to this hub instance.
    /// </summary>
    private readonly int _userId;

    /// <summary>
    /// A <c>Dictionary</c> contaning resources' information and the connection ids of
    /// the users connecting to each resource.
    /// </summary>
    /// <remarks>
    /// The key is an <c><see cref="Resource"/></c> contaning information of the
    /// resource.<br/>
    /// The value is a list contanining all the connection ids of the users connecting
    /// to the resource.
    /// </remarks>
    private static Dictionary<Resource, List<string>> _resourceConnectionDictionary;

    /// <summary>
    /// A <c>Dictionary</c> containing all the ids of the users connecting to the hub
    /// and the connection ids of each user.
    /// </summary>
    /// <remarks>
    /// The key is an <c><see cref="int"/></c> representing the id of each user.<br/>
    /// The value is a list contanining all the connection ids associated to the user.
    /// </remarks>
    private static Dictionary<int, List<string>> _userConnectionDictionary;

    /// <summary>
    /// Initializes a new instance of the <c><see cref="ApplicationHub"/></c> class
    /// for each connection handling.
    /// </summary>
    /// <param name="userService">
    /// An instance of the <c><see cref="IUserService"/></c>
    /// </param>
    public ApplicationHub(IUserService userService)
    {
        _userService = userService;
        _userId = int.Parse(Context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }

    /// <summary>
    /// Initializes the static members of the <c><see cref="ApplicationHub"/></c>
    /// </summary>
    static ApplicationHub()
    {
        _resourceConnectionDictionary = new Dictionary<Resource, List<string>>();
    }

    /// <summary>
    /// Handle when a user connects to the hub.
    /// </summary>
    /// <returns>
    /// A <c>Task</c> operation representing the async operation.
    /// </returns>
    public override async Task OnConnectedAsync()
    {
        // Check if the user is already been connecting to the hub with different
        // connection ids.
        _userConnectionDictionary.TryGetValue(_userId, out List<string> connectionIds);
        if (connectionIds == null)
        {
            connectionIds = new List<string>
            {
                Context.ConnectionId
            };
        }

        // Add the current connection id to the user's connection id list.
        _userConnectionDictionary.Add(_userId, connectionIds);

        await LogConnectionStatus(true);
    }
    
    /// <summary>
    /// Handle when a user disconnects from the hub.
    /// </summary>
    /// <param name="exception">
    /// The <c>Exception</c> storing the reason of the disconnection.
    /// </param>
    /// <returns>
    /// A <c>Task</c> operation representing the async operation.
    /// </returns>
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        // Remove the user id from the connection id dictionary if the list containing
        // the connection ids contans only 1 element.
        List<string> connectionIds = _userConnectionDictionary[_userId];
        if (connectionIds.Count == 1)
        {
            _userConnectionDictionary.Remove(_userId);
        }

        await LogConnectionStatus(false);
    }


    public async Task StartAccessResource(
            string resourceName,
            int resourcePrimaryId,
            int? resourceSecondaryId = null)
    {
        // Get the pair of resource and its connection ids.
        Resource resource = new Resource
        {
            Name = resourceName,
            PrimaryId = resourcePrimaryId,
            SecondaryId = resourceSecondaryId
        };

        _resourceConnectionDictionary
            .TryGetValue(resource, out List<string> connectionIds);
        if (connectionIds == null)
        {
            connectionIds = new List<string>();
        }

        // Add the current connection id to the resource connection id list.
        if (!connectionIds.Contains(Context.ConnectionId))
        {
            connectionIds.Add(Context.ConnectionId);
        }

        // Notify other accessing users that this user is connecting.
        UserBasicResponseDto userResponseDto = await _userService.GetBasicAsync(_userId);

        List<int> userIdsConnectingToThisResource;
        userIdsConnectingToThisResource = _userConnectionDictionary
            .Where(pair => pair.Value.Any(id => id.Contains())
        Console.WriteLine("UserIdList: " + string.Join(", ", _userList.UserIds));
        foreach (int accessingUserId in userList.UserIds.Where(id => id != userId))
        {
            await Clients.User(accessingUserId.ToString()).SendAsync(
                "ResourceAccessStarted",
                resourcePrimaryId,
                resourceSecondaryId,
                userResponseDto);
        }
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
}