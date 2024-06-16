namespace NATSInternal.Services.Tasks;

public class StatsTask : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IStatsTaskService _statsTaskService;
    private bool _isInitialExecution = true;

    public StatsTask(IServiceScopeFactory scopeFactory, IStatsTaskService statsTaskService)
    {
        _scopeFactory = scopeFactory;
        _statsTaskService = statsTaskService;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        DateTime nextExecutionDateTime;
        while (!cancellationToken.IsCancellationRequested)
        {
            using IServiceScope scope = _scopeFactory.CreateScope();

            // Get services.
            DatabaseContext context = scope.ServiceProvider
                .GetRequiredService<DatabaseContext>();
            DateOnly dateToBeClosed = DateOnly.FromDateTime(DateTime.Today);
            if (_isInitialExecution)
            {
                await CreateStatsRecordsAsync(context);
                if (DateTime.Now.Hour < 1)
                {
                    nextExecutionDateTime = new DateTime(
                        DateOnly.FromDateTime(DateTime.Today),
                        new TimeOnly(1, 0, 0));
                }
                else
                {
                    nextExecutionDateTime = new DateTime(
                        DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
                        new TimeOnly(1, 0, 0));

                    if (DateTime.Now.Hour >= 1 && DateTime.Now.Hour < 3)
                    {
                        await TemporarilyCloseAsync(
                            context,
                            DateOnly.FromDateTime(DateTime.Today.AddDays(-1)));
                        if (DateTime.Today.Day == 4)
                        {
                            await OfficiallyCloseAsync(context);
                        }
                    }
                }

                _isInitialExecution = false;
            }
            else
            {
                nextExecutionDateTime = new DateTime(
                    DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
                    new TimeOnly(1, 0, 0));
                if (nextExecutionDateTime.AddHours(1) < _statsTaskService.ExpectedRestartingDateTime)
                {
                    await TemporarilyCloseAsync(
                        context,
                        DateOnly.FromDateTime(DateTime.Today.AddDays(-1)));
                    if (DateTime.Today.Day == 4)
                    {
                        await OfficiallyCloseAsync(context);
                    }
                }
            }
            await Task.Delay(nextExecutionDateTime - DateTime.Now, cancellationToken);

        }
    }

    private static async Task TemporarilyCloseAsync(DatabaseContext context, DateOnly date)
    {
        DailyStats dailyStats = await context.DailyStats
            .Include(ds => ds.Monthly)
            .Where(ds => ds.RecordedDate == date)
            .SingleOrDefaultAsync();
        if (dailyStats != null && !dailyStats.IsOfficiallyClosed)
        {
            dailyStats.TemporarilyClosedDateTime = DateTime.Now;

            // Close month stats record if it's the first day of the month.
            if (dailyStats.RecordedDate.AddDays(1).Day == 1)
            {
                dailyStats.Monthly.TemporarilyClosedDateTime = DateTime.Now;
            }
            await context.SaveChangesAsync();
        }
    }

    private static async Task OfficiallyCloseAsync(DatabaseContext context)
    {
        // Determine datetime to be closed.
        await context.SaveChangesAsync();
        DateTime minDateTime = new DateTime(
            new DateOnly(
                DateTime.Today.AddMonths(-2).Year,
                DateTime.Today.AddMonths(-2).Month,
                1),
            new TimeOnly(0, 0, 0));
        DateTime maxDateTime = minDateTime.AddMonths(1);
        DateOnly dateToBeClosed = DateOnly.FromDateTime(minDateTime);

        // Close monthly and daily stats of the month.
        MonthlyStats monthlyStats = await context.MonthlyStats
            .Include(ms => ms.DailyStats)
            .Where(ms => ms.RecordedYear == dateToBeClosed.Year)
            .Where(ms => ms.RecordedMonth == dateToBeClosed.Month)
            .SingleOrDefaultAsync();
        if (monthlyStats != null && !monthlyStats.IsOfficiallyClosed)
        {
            monthlyStats.OfficiallyClosedDateTime = DateTime.Now;
            foreach (DailyStats dailyStats in monthlyStats.DailyStats)
            {
                dailyStats.OfficiallyClosedDateTime = monthlyStats.OfficiallyClosedDateTime;
            }

            // Close supplies.
            await context.Supplies
                .Where(s => s.SuppliedDateTime >= minDateTime)
                .Where(s => s.SuppliedDateTime < maxDateTime)
                .ExecuteUpdateAsync(setter => setter.SetProperty(s => s.IsClosed, true));

            // Close orders, order payments.
            await context.Orders
                .Where(o => o.OrderedDateTime >= minDateTime)
                .Where(o => o.OrderedDateTime < maxDateTime)
                .ExecuteUpdateAsync(setter => setter.SetProperty(o => o.IsClosed, true));
            await context.OrderPayments
                .Where(op => op.PaidDateTime >= minDateTime)
                .Where(op => op.PaidDateTime < maxDateTime)
                .ExecuteUpdateAsync(setter => setter.SetProperty(op => op.IsClosed, true));

            // Close treatments, treatment sessions, treatment payments.
            await context.Treatments
                .Where(t => t.OrderedDateTime >= minDateTime)
                .Where(t => t.OrderedDateTime < maxDateTime)
                .ExecuteUpdateAsync(setter => setter.SetProperty(t => t.IsClosed, true));
            await context.TreatmentSessions
                .Where(ts => ts.StartingDateTime >= minDateTime)
                .Where(ts => ts.StartingDateTime < maxDateTime)
                .ExecuteUpdateAsync(setter => setter.SetProperty(ts => ts.IsClosed, true));
            await context.TreatmentPayments
                .Where(tp => tp.PaidDateTime >= minDateTime)
                .Where(tp => tp.PaidDateTime < maxDateTime)
                .ExecuteUpdateAsync(setter => setter.SetProperty(tp => tp.IsClosed, true));

            // Close expenses.
            await context.Expenses
                .Where(e => e.PaidDateTime >= minDateTime)
                .Where(e => e.PaidDateTime < maxDateTime)
                .ExecuteUpdateAsync(setter => setter.SetProperty(t => t.IsClosed, true));
        }
    }

    private static async Task CreateStatsRecordsAsync(DatabaseContext context)
    {
        DateOnly minDate = DateOnly.FromDateTime(DateTime.Today);
        DateOnly lastCreatedDate = await context.DailyStats
            .OrderByDescending(ds => ds.RecordedDate)
            .Select(ds => ds.RecordedDate)
            .FirstOrDefaultAsync();

        DateOnly nextDateToBeCreated = minDate;
        if (lastCreatedDate != new DateOnly())
        {
            nextDateToBeCreated = lastCreatedDate.AddDays(1);
        }

        DateOnly currentCreatingDate = nextDateToBeCreated;

        while (currentCreatingDate <= minDate.AddDays(3))
        {
            DailyStats dailyStats = new DailyStats
            {
                CreatedDateTime = DateTime.Now,
                RecordedDate = currentCreatingDate
            };
            context.DailyStats.Add(dailyStats);

            MonthlyStats monthlyStats = await context.MonthlyStats
                .Where(ms => ms.RecordedYear == dailyStats.RecordedDate.Year)
                .Where(ms => ms.RecordedMonth == dailyStats.RecordedDate.Month)
                .SingleOrDefaultAsync();
            if (monthlyStats == null)
            {
                monthlyStats = new MonthlyStats
                {
                    CreatedDateTime = DateTime.Now,
                    RecordedYear = dailyStats.RecordedDate.Year,
                    RecordedMonth = dailyStats.RecordedDate.Month
                };
                context.MonthlyStats.Add(monthlyStats);
            }

            dailyStats.Monthly = monthlyStats;

            currentCreatingDate = currentCreatingDate.AddDays(1);

            await context.SaveChangesAsync();
        }
    }
}
