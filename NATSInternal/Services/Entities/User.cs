namespace NATSInternal.Services.Entities;

[Table("users")]
public class User : IdentityUser<int>
{
    [Column("first_name")]
    [Required]
    [StringLength(10)]
    public required string FirstName { get; set; }

    [Column("normalized_first_name")]
    [Required]
    [StringLength(10)]
    public required string NormalizedFirstName { get; set; }

    [Column("middle_name")]
    [StringLength(20)]
    public string MiddleName { get; set; }

    [Column("normalized_middle_name")]
    [StringLength(20)]
    public string NormalizedMiddleName { get; set; }

    [Column("last_name")]
    [Required]
    [StringLength(10)]
    public required string LastName { get; set; }

    [Column("normalized_last_name")]
    [Required]
    [StringLength(10)]
    public required string NormalizedLastName { get; set; }


    [Column("full_name")]
    [Required]
    [StringLength(45)]
    public required string FullName { get; set; }

    [Column("normalized_full_name")]
    [Required]
    [StringLength(45)]
    public required string NormalizedFullName { get; set; }

    [Column("gender")]
    [Required]
    public Gender Gender { get; set; }

    [Column("birthday")]
    public DateOnly? Birthday { get; set; }

    [Column("joining_date")]
    public DateOnly? JoiningDate { get; set; }

    [Column("created_datetime")]
    [Required]
    public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow.ToApplicationTime();

    [Column("updated_datetime")]
    public DateTime? UpdatedDateTime { get; set; }

    [Column("note")]
    [StringLength(255)]
    public string Note { get; set; }

    [Column("avatar_url")]
    [StringLength(255)]
    public string AvatarUrl { get; set; }

    [Column("is_deleted")]
    [Required]
    public bool IsDeleted { get; set; } = false;

    [Timestamp]
    public byte[] RowVersion { get; set; }

    // Navigation properties.
    public virtual List<Role> Roles { get; set; }
    public virtual List<UserRefreshToken> RefreshTokens { get; set; }
    public virtual List<Customer> CreatedCustomers { get; set; }
    public virtual List<Supply> Supplies { get; set; }
    public virtual List<SupplyUpdateHistory> SupplyUpdateHistories { get; set; }
    public virtual List<Order> Orders { get; set; }
    public virtual List<OrderUpdateHistory> OrderUpdateHistories { get; set; }
    public virtual List<Treatment> CreatedTreatments { get; set; }
    public virtual List<Treatment> TreatmentsInCharge { get; set; }
    public virtual List<TreatmentUpdateHistory> TreatmentUpdateHistories { get; set; }
    public virtual List<Consultant> Consultants { get; set; }
    public virtual List<ConsultantUpdateHistory> ConsultantUpdateHistories { get; set; }
    public virtual List<Expense> Expenses { get; set; }
    public virtual List<ExpenseUpdateHistory> ExpenseUpdateHistories { get; set; }
    public virtual List<Debt> Debts { get; set; }
    public virtual List<DebtUpdateHistory> DebtUpdateHistories { get; set; }
    public virtual List<DebtPayment> DebtPayments { get; set; }
    public virtual List<DebtPaymentUpdateHistory> DebtPaymentUpdateHistories { get; set; }
    public virtual List<Announcement> CreatedAnnouncements { get; set; }
    public virtual List<Notification> ReceivedNotifications { get; set; }
    public virtual List<Notification> ReadNotifications { get; set; }

    // Properties for convinience.
    [NotMapped]
    public Role Role => Roles.OrderByDescending(r => r.PowerLevel).SingleOrDefault();

    [NotMapped]
    public int PowerLevel => Role.PowerLevel;

    [NotMapped]
    public List<Supply> UpdatedSupplies => SupplyUpdateHistories.Select(suh => suh.Supply).ToList();

    [NotMapped]
    public List<Expense> UpdatedExpenses => ExpenseUpdateHistories.Select(euh => euh.Expense).ToList();

    [NotMapped]
    public List<Order> UpdatedOrders => OrderUpdateHistories.Select(ouh => ouh.Order).ToList();

    [NotMapped]
    public List<Treatment> UpdatedTreatments => TreatmentUpdateHistories.Select(tuh => tuh.Treatment).ToList();

    [NotMapped]
    public List<Debt> UpdatedDebts => DebtUpdateHistories.Select(duh => duh.Debt).ToList();

    [NotMapped]
    public List<DebtPayment> UpdatedDebtPayments => DebtPaymentUpdateHistories
        .Select(dpuh => dpuh.DebtPayment)
        .ToList();

    public bool HasPermission(string permissionName)
    {
        return Role.Claims.Any(r => r.ClaimType == "Permission" && r.ClaimValue == permissionName);
    }
}