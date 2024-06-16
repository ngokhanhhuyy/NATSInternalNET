namespace NATSInternal.Services.Entities;

public class DailyStats
{
    [Column("id")]
    [Key]
    public long Id { get; set; }

    [Column("retail_revenue")]
    [Required]
    public long RetailRevenue { get; set; }

    [Column("treatment_revenue")]
    [Required]
    public long TreatmentRevenue { get; set; }

    [Column("consultant_revenue")]
    [Required]
    public long ConsultantRevenue { get; set; }

    [Column("shipment_cost")]
    [Required]
    public long ShipmentCost { get; set; }

    [Column("supply_expense")]
    [Required]
    public long SupplyCost { get; set; }

    [Column("utilities_expenses")]
    [Required]
    public long UtilitiesExpenses { get; set; }

    [Column("equipment_expenses")]
    [Required]
    public long EquipmentExpenses { get; set; }

    [Column("office_expese")]
    [Required]
    public long OfficeExpense { get; set; }

    [Column("staff_expense")]
    [Required]
    public long StaffExpense { get; set; }

    [Column("recorded_date")]
    [Required]
    public DateOnly RecordedDate { get; set; }

    [Column("created_datetime")]
    [Required]
    public DateTime CreatedDateTime { get; set; }

    [Column("temporarily_closed_datetime")]
    public DateTime? TemporarilyClosedDateTime { get; set; }

    [Column("officially_closed_datetime")]
    public DateTime? OfficiallyClosedDateTime { get; set; }

    // Foreign key.
    [Column("monthly_stats_id")]
    [Required]
    public long MonthlyStatsId { get; set; }

    // Navigation properties.
    public virtual MonthlyStats Monthly { get; set; }

    // Properties for convinience.
    [NotMapped]
    public long Cost => SupplyCost + ShipmentCost;

    [NotMapped]
    public long Expenses => UtilitiesExpenses + EquipmentExpenses + OfficeExpense + StaffExpense;

    [NotMapped]
    public long Revenue => RetailRevenue + TreatmentRevenue + ConsultantRevenue;

    [NotMapped]
    public long NetProfit => Revenue - (Cost + Expenses);

    [NotMapped]
    public long GrossProfit => Revenue - Cost;

    [NotMapped]
    public long OperatingProfit => Revenue - Expenses;

    [NotMapped]
    public bool IsTemporarilyClosed => TemporarilyClosedDateTime.HasValue;

    [NotMapped]
    public bool IsOfficiallyClosed => OfficiallyClosedDateTime.HasValue;
}
