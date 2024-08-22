namespace NATSInternal.Services.Dtos;

public class DailyStatsDetailResponseDto : StatsDetailResponseDto
{
    public DateOnly RecordedDate { get; set; }

    public DailyStatsDetailResponseDto(DailyStats dailyStats)
    {
        RetailGrossRevenue = dailyStats.RetailGrossRevenue;
        TreatmentGrossRevenue = dailyStats.TreatmentGrossRevenue;
        ConsultantGrossRevenue = dailyStats.ConsultantGrossRevenue;
        VatCollectedAmount = dailyStats.VatCollectedAmount;
        DebtAmount = dailyStats.DebtIncurredAmount;
        DebtPaidAmount = dailyStats.DebtPaidAmount;
        ShipmentCost = dailyStats.ShipmentCost;
        SupplyCost = dailyStats.SupplyCost;
        UtilitiesExpenses = dailyStats.UtilitiesExpenses;
        EquipmentExpenses = dailyStats.EquipmentExpenses;
        OfficeExpense = dailyStats.OfficeExpense;
        StaffExpense = dailyStats.StaffExpense;
        Cost = dailyStats.Cost;
        Expenses = dailyStats.Expenses;
        GrossRevenue = dailyStats.GrossRevenue;
        NetRevenue = dailyStats.NetRevenue;
        DebtAmount = dailyStats.DebtAmount;
        GrossProfit = dailyStats.GrossProfit;
        NetProfit = dailyStats.NetProfit;
        OperatingProfit = dailyStats.OperatingProfit;
        TemporarilyClosedDateTime = dailyStats.TemporarilyClosedDateTime;
        OfficiallyClosedDateTime = dailyStats.OfficiallyClosedDateTime;
        RecordedDate = dailyStats.RecordedDate;
    }
}