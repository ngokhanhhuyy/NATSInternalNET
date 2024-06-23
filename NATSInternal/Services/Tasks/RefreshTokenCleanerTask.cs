namespace NATSInternal.Services.Tasks;

public class RefreshTokenCleanerTask : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public RefreshTokenCleanerTask(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using (IServiceScope scope = _scopeFactory.CreateScope())
        {
            DatabaseContext context = scope.ServiceProvider
                .GetRequiredService<DatabaseContext>();
            await CleanExpiredRefreshTokens(context);
        }
    }

    private static async Task CleanExpiredRefreshTokens(DatabaseContext context)
    {
        DateTime currentDateTime = DateTime.UtcNow.ToApplicationTime();
        List<UserRefreshToken> expiredRefreshTokens = await context.UserRefreshTokens
            .Where(t => t.ExpiringDateTime < currentDateTime)
            .ToListAsync();

        foreach (UserRefreshToken refreshToken in expiredRefreshTokens)
        {
            context.UserRefreshTokens.Remove(refreshToken);
        }

        await context.SaveChangesAsync();
    }
}