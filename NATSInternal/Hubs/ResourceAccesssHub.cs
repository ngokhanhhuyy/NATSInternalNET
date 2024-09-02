using System.Globalization;
using System.Security.Claims;

namespace NATSInternal.Hubs;

[Authorize]
public class ResourceAccessingUsersHub : Hub
{
    private readonly IUserService _userService;
    private static AccessingResourceDictionary _resourceAccessingData;
    private static List<UserConnectionList> _userConnectionLists;

    public ResourceAccessingUsersHub(IUserService userService)
    {
        _userService = userService;
    }

    static ResourceAccessingUsersHub()
    {
        _resourceAccessingData = new AccessingResourceDictionary
        {
            { nameof(User), new List<ResourceAccessingConnectionIdList>() },
            { nameof(Customer), new List<ResourceAccessingConnectionIdList>() },
            { nameof(Brand), new List<ResourceAccessingConnectionIdList>() },
            { nameof(Product), new List<ResourceAccessingConnectionIdList>() },
            { nameof(ProductCategory), new List<ResourceAccessingConnectionIdList>() },
            { nameof(Supply), new List<ResourceAccessingConnectionIdList>() },
            { nameof(Expense), new List<ResourceAccessingConnectionIdList>() },
            { nameof(Order), new List<ResourceAccessingConnectionIdList>() },
            { nameof(Treatment), new List<ResourceAccessingConnectionIdList>() },
            { nameof(Consultant), new List<ResourceAccessingConnectionIdList>() },
            { nameof(DebtIncurrence), new List<ResourceAccessingConnectionIdList>() },
            { nameof(DebtPayment), new List<ResourceAccessingConnectionIdList>() },
            { nameof(Announcement), new List<ResourceAccessingConnectionIdList>() },
        };
        
        _userConnectionLists = new List<UserConnectionList>();
    }

    public override async Task OnConnectedAsync()
    {
        await LogConnectionStatus(true);
    }

    public override async Task OnDisconnectedAsync(Exception _)
    {
        await LogConnectionStatus(false);
    }

    public async Task StartAccessResource(
            string resourceName,
            int resourcePrimaryId,
            int? resourceSecondaryId = null)
    {
        // Retrieve the user id in the authentication credentials.
        int userId = int.Parse(Context.User
            .FindFirst(ClaimTypes.NameIdentifier)!.Value);
        
        // Check if the requested resource name valid.
        bool isResourceNameValid = _resourceAccessingData.TryGetValue(
            resourceName,
            out List<ResourceAccessingConnectionIdList> resourceAccessingUserLists);
        if (!isResourceNameValid)
        {
            throw new ArgumentException();
        }

        // Get the list of resources containing accessing user ids.
        ResourceAccessingConnectionIdList userList = resourceAccessingUserLists
            .Where(ul => ul.ResourcePrimaryId == resourcePrimaryId)
            .Where(ul => ul.ResourceSecondaryId == resourceSecondaryId)
            .SingleOrDefault();

        if (userList == null)
        {
            userList = new ResourceAccessingConnectionIdList
            {
                ResourcePrimaryId = resourcePrimaryId,
                ResourceSecondaryId = resourceSecondaryId
            };
            resourceAccessingUserLists.Add(userList);
        }

        // Add the user id to the list.
        userList.UserIds.Add(userId);

        // Notify other accessing users that this user is connecting.
        UserBasicResponseDto userResponseDto = await _userService.GetBasicAsync(userId);
        Console.WriteLine("UserList: " + string.Join(", ", userList.UserIds));
        foreach (int accessingUserId in userList.UserIds.Where(id => id != userId))
        {
            await Clients.User(accessingUserId.ToString()).SendAsync(
                "ResourceAccessStarted",
                resourcePrimaryId,
                resourceSecondaryId,
                userResponseDto);
        }
    }

    public async Task FinishAccessResource() {}
    
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
            $" {connectionStatus} ResourceAccessHub ({userName}#{userId})");
    }

    /// <summary>
    /// A <c>Dictionary</c> which each element's key is to store the resource name and the
    /// value is to store the a resource information and all connection ids of the users
    /// connecting to it.
    /// </summary>
    private class AccessingResourceDictionary
        : Dictionary<string, List<ResourceAccessingConnectionIdList>> { }

    /// <summary>
    /// A data class to store a resource's ids and all connection id
    /// </summary>
    private class ResourceAccessingConnectionIdList
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

    /// <summary>
    /// A data class to store a user's id and all connection ids.
    /// </summary>
    private class UserConnectionList
    {
        public int UserId { get; set; }
        public List<string> UserConnectionIds { get; set; }
    }
}