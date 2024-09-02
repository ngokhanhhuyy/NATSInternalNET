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
            { nameof(User), new List<ResourceAccessConnectionIdList>() },
            { nameof(Customer), new List<ResourceAccessConnectionIdList>() },
            { nameof(Brand), new List<ResourceAccessConnectionIdList>() },
            { nameof(Product), new List<ResourceAccessConnectionIdList>() },
            { nameof(ProductCategory), new List<ResourceAccessConnectionIdList>() },
            { nameof(Supply), new List<ResourceAccessConnectionIdList>() },
            { nameof(Expense), new List<ResourceAccessConnectionIdList>() },
            { nameof(Order), new List<ResourceAccessConnectionIdList>() },
            { nameof(Treatment), new List<ResourceAccessConnectionIdList>() },
            { nameof(Consultant), new List<ResourceAccessConnectionIdList>() },
            { nameof(DebtIncurrence), new List<ResourceAccessConnectionIdList>() },
            { nameof(DebtPayment), new List<ResourceAccessConnectionIdList>() },
            { nameof(Announcement), new List<ResourceAccessConnectionIdList>() },
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
            out List<ResourceAccessConnectionIdList> resourceAccessingUserLists);
        if (!isResourceNameValid)
        {
            throw new ArgumentException();
        }

        // Get the list of resources containing accessing user ids.
        ResourceAccessConnectionIdList userList = resourceAccessingUserLists
            .Where(ul => ul.ResourcePrimaryId == resourcePrimaryId)
            .Where(ul => ul.ResourceSecondaryId == resourceSecondaryId)
            .SingleOrDefault();

        if (userList == null)
        {
            userList = new ResourceAccessConnectionIdList
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
        : Dictionary<string, List<ResourceAccessConnectionIdList>> { }



    /// <summary>
    /// A data class to store a user's id and all connection ids.
    /// </summary>
    private class UserConnectionList
    {
        public int UserId { get; set; }
        public List<string> UserConnectionIds { get; set; }
    }
}