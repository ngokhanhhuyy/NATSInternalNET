namespace NATSInternal.Services;

/// <inheritdoc />
public class StatsService : IStatsService
{
    private readonly DatabaseContext _context;

    public StatsService(DatabaseContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<MonthlyStatsDetailResponseDto> GetMonthlyStatsDetailAsync(
            MonthlyStatsRequestDto requestDto)
    {
        IQueryable<MonthlyStats> query = _context.MonthlyStats.Include(m => m.DailyStats);
        int recordedMonth;
        int recordedYear;
        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow.ToApplicationTime());
        if (requestDto == null)
        {
            recordedMonth = today.Month;
            recordedYear = today.Year;
        }
        else
        {
            recordedMonth = requestDto.RecordedMonth == 0
                ? today.Month
                : requestDto.RecordedMonth;
            recordedYear = requestDto.RecordedYear == 0
                ? today.Year
                : requestDto.RecordedYear;
        }

        MonthlyStats stats = await _context.MonthlyStats
            .Include(ms => ms.DailyStats)
            .SingleOrDefaultAsync(ms => ms.RecordedYear == recordedYear && ms.RecordedMonth == recordedMonth)
            ?? throw new ResourceNotFoundException(
                nameof(MonthlyStats),
                nameof(DisplayNames.RecordedMonthAndYear),
                $"Tháng {recordedMonth} năm {recordedYear}");
        return ConvertToMonthlyDetailResponseDto(stats);
    }

    /// <inheritdoc />
    public async Task<DailyStatsDetailResponseDto> GetDailyStatsDetailAsync(DateOnly? recordedDate)
    {
        DateTime currentDateTime = DateTime.UtcNow.ToApplicationTime();
        DateOnly date = recordedDate ?? DateOnly.FromDateTime(currentDateTime);
        return await _context.DailyStats
            .Where(d => d.RecordedDate == date)
            .Select(d => ConvertToDailyDetailResponseDto(d))
            .SingleOrDefaultAsync()
            ?? throw new ResourceNotFoundException(
                nameof(DailyStats),
                nameof(date),
                date.ToVietnameseString());
    }

    /// <inheritdoc />
    public async Task IncrementRetailGrossRevenueAsync(long value, DateOnly? date = null)
    {
        DailyStats dailyStats = await FetchStatisticsEntitiesAsync(date);
        dailyStats.RetailGrossRevenue += value;
        dailyStats.Monthly.RetailGrossRevenue += value;
        await _context.SaveChangesAsync();
    }
    
    /// <inheritdoc />
    public async Task IncrementTreatmentGrossRevenueAsync(long value, DateOnly? date = null)
    {
        DailyStats dailyStats = await FetchStatisticsEntitiesAsync(date);
        dailyStats.TreatmentGrossRevenue += value;
        dailyStats.Monthly.TreatmentGrossRevenue += value;
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task IncrementConsultantGrossRevenueAsync(long value, DateOnly? date = null)
    {
        DailyStats dailyStats = await FetchStatisticsEntitiesAsync(date);
        dailyStats.ConsultantGrossRevenue += value;
        dailyStats.Monthly.ConsultantGrossRevenue += value;
        await _context.SaveChangesAsync();
    }
    
    /// <inheritdoc />
    public async Task IncrementDebtAmountAsync(long value, DateOnly? date = null)
    {
        DailyStats dailyStats = await FetchStatisticsEntitiesAsync(date);
        dailyStats.DebtAmount += value;
        dailyStats.Monthly.DebtAmount += value;
        await _context.SaveChangesAsync();
    }
    
    /// <inheritdoc />
    public async Task IncrementDebtPaidAmountAsync(long value, DateOnly? date = null)
    {
        DailyStats dailyStats = await FetchStatisticsEntitiesAsync(date);
        dailyStats.DebtPaidAmount += value;
        dailyStats.Monthly.DebtPaidAmount += value;
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task IncrementVatCollectedAmountAsync(long amount, DateOnly? date = null)
    {
        DailyStats dailyStats = await FetchStatisticsEntitiesAsync(date);
        dailyStats.VatCollectedAmount += amount;
        dailyStats.Monthly.VatCollectedAmount += amount;
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task IncrementShipmentCostAsync(long value, DateOnly? date = null)
    {
        DailyStats dailyStats = await FetchStatisticsEntitiesAsync(date);
        dailyStats.ShipmentCost += value;
        dailyStats.Monthly.ShipmentCost += value;
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task IncrementSupplyCostAsync(long value, DateOnly? date = null)
    {
        DailyStats dailyStats = await FetchStatisticsEntitiesAsync(date);
        dailyStats.SupplyCost += value;
        dailyStats.Monthly.SupplyCost += value;
        await _context.SaveChangesAsync();
    }
    
    /// <inheritdoc />
    public async Task IncrementExpenseAsync(
            long value, 
            ExpenseCategory category,
            DateOnly? date = null)
    {
        DailyStats dailyStats = await FetchStatisticsEntitiesAsync(date);
        switch (category)
        {
            case ExpenseCategory.Equipment:
                dailyStats.EquipmentExpenses += value;
                dailyStats.Monthly.EquipmentExpenses += value;
                break;
            case ExpenseCategory.Office:
                dailyStats.OfficeExpense += value;
                dailyStats.Monthly.OfficeExpense += value;
                break;
            case ExpenseCategory.Utilities:
                dailyStats.UtilitiesExpenses += value;
                dailyStats.Monthly.UtilitiesExpenses += value;
                break;
            default:
            case ExpenseCategory.Staff:
                dailyStats.StaffExpense += value;
                dailyStats.Monthly.StaffExpense += value;
                break;
        }
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task TemporarilyCloseAsync(DateOnly date)
    {
        DailyStats dailyStats = await FetchStatisticsEntitiesAsync(date);
        dailyStats.TemporarilyClosedDateTime = DateTime.UtcNow.ToApplicationTime();
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public bool VerifyResourceDateTimeToBeCreated(DateTime dateTime)
    {
        DateTime currentDateTime = DateTime.UtcNow.ToApplicationTime();
        DateTime resourceMinimumOpenedDateTime = GetResourceMinimumOpenedDateTime();

        return currentDateTime > resourceMinimumOpenedDateTime;
    }

    /// <inheritdoc />
    public bool VerifyResourceDateTimeToBeUpdated(DateTime originalDateTime, DateTime newDateTime)
    {
        bool isOriginalDateTimeClosed = originalDateTime < GetResourceMinimumOpenedDateTime();
        bool isNewDateTimeClosed = newDateTime < GetResourceMinimumOpenedDateTime();
        return isOriginalDateTimeClosed == isNewDateTimeClosed;
    }

    /// <inheritdoc />
    public DateTime GetResourceMinimumOpenedDateTime()
    {
        DateTime currentDateTime = DateTime.UtcNow.ToApplicationTime();
        DateTime lastMonthEditableMaxDateTime = new DateTime(
            currentDateTime.Year, currentDateTime.Month, 4,
            2, 0, 0);
        if (currentDateTime < lastMonthEditableMaxDateTime)
        {
            return new DateTime(
                currentDateTime.AddMonths(-2).Year,
                currentDateTime.AddMonths(-2).Month,
                1,
                0, 0, 0);
        }

        return new DateTime(
            currentDateTime.AddMonths(-1).Year,
            currentDateTime.AddMonths(-1).Month,
            1,
            0, 0, 0);
    }

    /// <summary>
    /// Get the daily stats entity by the given recorded date. If the recorded date
    /// is not specified, the date will be today's date value.
    /// </summary>
    /// <param name="date">
    /// Optional - The date when the stats entity is recorded. If not specified,
    /// the entity will be fetched based on today's date.</param>
    /// <returns>The DailyStats entity.</returns>
    private async Task<DailyStats> FetchStatisticsEntitiesAsync(DateOnly? date = null)
    {
        DateOnly dateValue = date ?? DateOnly.FromDateTime(DateTime.UtcNow.ToApplicationTime());
        DailyStats dailyStats = await _context.DailyStats
            .Include(ds => ds.Monthly)
            .Where(ds => ds.RecordedDate == dateValue)
            .SingleOrDefaultAsync();

        if (dailyStats == null)
        {
            dailyStats = new DailyStats
            {
                RecordedDate = dateValue,
                CreatedDateTime = DateTime.UtcNow.ToApplicationTime(),
            };
            _context.DailyStats.Add(dailyStats);

            MonthlyStats monthlyStats = await _context.MonthlyStats
                .Where(ms => ms.RecordedYear == dateValue.Year)
                .Where(ms => ms.RecordedMonth == dateValue.Month)
                .SingleOrDefaultAsync();
            if (monthlyStats == null)
            {
                monthlyStats = new MonthlyStats
                {
                    RecordedMonth = dateValue.Month,
                    RecordedYear = dateValue.Year
                };
                _context.MonthlyStats.Add(monthlyStats);
            }

            dailyStats.Monthly = monthlyStats;
            await _context.SaveChangesAsync();
        }

        return dailyStats;
    }

    /// <summary>
    /// Convert a daily stats entity into a basic response dto object.
    /// </summary>
    /// <param name="stats">A DailyStats entity.</param>
    /// <returns>A <c>DailyStatsDetailResponseDto</c> object.</returns>
    private static DailyStatsBasicResponseDto ConvertToDailyBasicResponseDto(DailyStats stats)
    {
        return new DailyStatsBasicResponseDto
        {
            Cost = stats.Cost,
            Expenses = stats.Expenses,
            GrossRevenue = stats.GrossRevenue,
            NetRevenue = stats.NetRevenue,
            NetProfit = stats.NetProfit,
            RecordedDate = stats.RecordedDate,
            IsTemporarilyClosed = stats.IsTemporarilyClosed,
            IsOfficiallyClosed = stats.IsOfficiallyClosed,
        };
    }

    /// <summary>
    /// Convert a daily stats entity into a detail response dto object.
    /// </summary>
    /// <param name="stats">A DailyStats entity.</param>
    /// <returns>A <c>DailyStatsDetailResponseDto</c> object.</returns>
    private static DailyStatsDetailResponseDto ConvertToDailyDetailResponseDto(DailyStats stats)
    {
        return new DailyStatsDetailResponseDto
        {
            RetailGrossRevenue = stats.RetailGrossRevenue,
            TreatmentGrossRevenue = stats.TreatmentGrossRevenue,
            ConsultantGrossRevenue = stats.ConsultantGrossRevenue,
            VatCollectedAmount = stats.VatCollectedAmount,
            DebtAmount = stats.DebtAmount,
            DebtPaidAmount = stats.DebtPaidAmount,
            ShipmentCost = stats.ShipmentCost,
            SupplyCost = stats.SupplyCost,
            UtilitiesExpenses = stats.UtilitiesExpenses,
            EquipmentExpenses = stats.EquipmentExpenses,
            OfficeExpense = stats.EquipmentExpenses,
            StaffExpense = stats.StaffExpense,
            Cost = stats.Cost,
            Expenses = stats.Expenses,
            GrossRevenue = stats.GrossRevenue,
            NetRevenue = stats.NetRevenue,
            RemainingDebtAmount = stats.RemainingDebtAmount,
            GrossProfit = stats.GrossProfit,
            NetProfit = stats.NetProfit,
            OperatingProfit = stats.OperatingProfit,
            RecordedDate = stats.RecordedDate,
            TemporarilyClosedDateTime = stats.TemporarilyClosedDateTime,
            OfficiallyClosedDateTime = stats.OfficiallyClosedDateTime,
        };
    }

    /// <summary>
    /// Convert a monthly stats entity into a basic response dto object.
    /// </summary>
    /// <param name="stats">A MonthlyStats entity.</param>
    /// <returns>A <c>MonthlyStatsDetailResponseDto</c> object.</returns>
    private static MonthlyStatsBasicResponseDto ConvertToMonthlyBasicResponseDto(MonthlyStats stats)
    {
        return new MonthlyStatsBasicResponseDto
        {
            Cost = stats.Cost,
            Expenses = stats.Expenses,
            GrossRevenue = stats.GrossRevenue,
            NetRevenue = stats.NetRevenue,
            NetProfit = stats.NetProfit,
            RecordedMonth = stats.RecordedMonth,
            RecordedYear = stats.RecordedYear,
            IsTemporarilyClosed = stats.IsTemporarilyClosed,
            IsOfficiallyClosed = stats.IsOfficiallyClosed,
        };
    }

    /// <summary>
    /// Convert a monthly stats entity into a detail response dto object.
    /// </summary>
    /// <param name="stats">A MonthlyStats entity.</param>
    /// <returns>A <c>MonthlyStatsDetailResponseDto</c> object.</returns>
    private static MonthlyStatsDetailResponseDto ConvertToMonthlyDetailResponseDto(MonthlyStats stats)
    {
        DateOnly currentDate = DateOnly.FromDateTime(DateTime.UtcNow.ToApplicationTime());
        return new MonthlyStatsDetailResponseDto
        {
            RetailGrossRevenue = stats.RetailGrossRevenue,
            TreatmentGrossRevenue = stats.TreatmentGrossRevenue,
            ConsultantGrossRevenue = stats.ConsultantGrossRevenue,
            VatCollectedAmount = stats.VatCollectedAmount,
            DebtAmount = stats.DebtAmount,
            DebtPaidAmount = stats.DebtPaidAmount,
            ShipmentCost = stats.ShipmentCost,
            SupplyCost = stats.SupplyCost,
            UtilitiesExpenses = stats.UtilitiesExpenses,
            EquipmentExpenses = stats.EquipmentExpenses,
            OfficeExpense = stats.EquipmentExpenses,
            StaffExpense = stats.StaffExpense,
            Cost = stats.Cost,
            Expenses = stats.Expenses,
            GrossRevenue = stats.GrossRevenue,
            NetRevenue = stats.NetRevenue,
            RemainingDebtAmount = stats.RemainingDebtAmount,
            GrossProfit = stats.GrossProfit,
            NetProfit = stats.NetProfit,
            OperatingProfit = stats.OperatingProfit,
            RecordedMonth = stats.RecordedMonth,
            RecordedYear = stats.RecordedYear,
            TemporarilyClosedDateTime = stats.TemporarilyClosedDateTime,
            OfficiallyClosedDateTime = stats.OfficiallyClosedDateTime,
            DailyStats = stats.DailyStats
                .Where(ds => ds.RecordedDate <= currentDate)
                .Select(ds => ConvertToDailyBasicResponseDto(ds))
                .ToList()
        };
    }
}
