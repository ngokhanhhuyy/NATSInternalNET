namespace NATSInternal.Services.Entities;

[Table("debts")]
public class Debt
{
    [Column("id")]
    [Key]
    public int Id { get; set; }

    [Column("amount")]
    [Required]
    public long Amount { get; set; }

    [Column("note")]
    [StringLength(255)]
    public string Note { get; set; }

    [Column("created_datetime")]
    [Required]
    public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow.ToApplicationTime();

    [Column("is_closed")]
    [Required]
    public bool IsClosed { get; set; }

    [Column("is_deleted")]
    [Required]
    public bool IsDeleted { get; set; }

    // Foreign keys.
    [Column("customer_id")]
    [Required]
    public int CustomerId { get; set; }

    [Column("created_user_id")]
    [Required]
    public int CreatedUserId { get; set; }

    // Navigation properties.
    public virtual Customer Customer { get; set; }
    public virtual User CreatedUser { get; set; }
    public virtual List<DebtUpdateHistory> UpdateHistories { get; set; }

    [NotMapped]
    public DateTime? LastUpdatedDateTime => UpdateHistories
        .OrderBy(uh => uh.UpdatedDateTime)
        .Select(uh => uh.UpdatedDateTime)
        .LastOrDefault();

    [NotMapped]
    public User LastUpdatedUser => UpdateHistories
        .OrderBy(uh => uh.UpdatedDateTime)
        .Select(uh => uh.User)
        .LastOrDefault();
}
