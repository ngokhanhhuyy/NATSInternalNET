using NATSInternal.Services.Entities;

namespace NATSInternal.Services.Dtos;

public class MonthlyStatsDetailResponseDto : StatsDetailResponseDto
{
    public int RecordedYear { get; set; }
    public int RecordedMonth { get; set; }
    public List<DailyStatsBasicResponseDto> DailyStats { get; set; }

    public MonthlyStatsDetailResponseDto(MonthlyStats monthlyStats)
    {
        RetailGrossRevenue = monthlyStats.RetailGrossRevenue;
        TreatmentGrossRevenue = monthlyStats.TreatmentGrossRevenue;
        ConsultantGrossRevenue = monthlyStats.ConsultantGrossRevenue;
        VatCollectedAmount = monthlyStats.VatCollectedAmount;
        DebtAmount = monthlyStats.DebtAmount;
        DebtPaidAmount = monthlyStats.DebtPaidAmount;
        ShipmentCost = monthlyStats.ShipmentCost;
        SupplyCost = monthlyStats.SupplyCost;
        UtilitiesExpenses = monthlyStats.UtilitiesExpenses;
        EquipmentExpenses = monthlyStats.EquipmentExpenses;
        OfficeExpense = monthlyStats.OfficeExpense;
        StaffExpense = monthlyStats.StaffExpense;
        Cost = monthlyStats.Cost;
        Expenses = monthlyStats.Expenses;
        GrossRevenue = monthlyStats.GrossRevenue;
        NetRevenue = monthlyStats.NetRevenue;
        RemainingDebtAmount = monthlyStats.RemainingDebtAmount;
        GrossProfit = monthlyStats.GrossProfit;
        NetProfit = monthlyStats.NetProfit;
        OperatingProfit = monthlyStats.OperatingProfit;
        TemporarilyClosedDateTime = monthlyStats.TemporarilyClosedDateTime;
        OfficiallyClosedDateTime = monthlyStats.OfficiallyClosedDateTime;
        RecordedYear = monthlyStats.RecordedYear;
        RecordedMonth = monthlyStats.RecordedMonth;
        DailyStats = monthlyStats.DailyStats?
            .Select(ds => new DailyStatsBasicResponseDto(ds))
            .ToList();
    }
}