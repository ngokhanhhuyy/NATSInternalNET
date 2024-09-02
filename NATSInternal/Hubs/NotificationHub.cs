using System.Globalization;
using System.Security.Claims;

namespace NATSInternal.Hubs;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ApplicationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await LogConnectionStatus(true);
    }
    
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        await LogConnectionStatus(false);
    }
    
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