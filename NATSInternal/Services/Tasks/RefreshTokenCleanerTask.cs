namespace NATSInternal.Services.Tasks;

public class RefreshTokenCleanerTask : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly DateTime _startedDateTime;
    private readonly DateTime _expectedRestartingDateTime;
    private bool _isInitialExecution = true;

    public RefreshTokenCleanerTask(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        _startedDateTime = DateTime.Now;
        _expectedRestartingDateTime = _startedDateTime.AddHours(29);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        DateTime nextExecutionDateTime;
        while (!cancellationToken.IsCancellationRequested)
        {
            using (IServiceScope scope = _scopeFactory.CreateScope())
            {
                DatabaseContext context = scope.ServiceProvider
                    .GetRequiredService<DatabaseContext>();
                if (_isInitialExecution)
                {
                    nextExecutionDateTime = _startedDateTime.AddMinutes(30);
                    _isInitialExecution = false;
                    await CleanExpiredRefreshTokens(context);
                }
                else
                {
                    if (DateTime.Now < _expectedRestartingDateTime.AddMinutes(-10))
                    {
                        nextExecutionDateTime = _startedDateTime.AddMinutes(60);
                        await CleanExpiredRefreshTokens(context);
                    }
                    else
                    {
                        nextExecutionDateTime = _startedDateTime.AddMinutes(120);
                    }
                }
            }

            await Task.Delay(nextExecutionDateTime - DateTime.Now, cancellationToken);
        }
    }

    private static async Task CleanExpiredRefreshTokens(DatabaseContext context)
    {
        List<UserRefreshToken> expiredRefreshTokens = await context.UserRefreshTokens
            .Where(t => t.ExpiringDateTime < DateTime.Now)
            .ToListAsync();

        foreach (UserRefreshToken refreshToken in expiredRefreshTokens)
        {
            context.UserRefreshTokens.Remove(refreshToken);
        }

        await context.SaveChangesAsync();
    }
}